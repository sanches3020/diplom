using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.DTO.Psychologist;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.PsychologistArea;

namespace Sofia.Web.Services;

public class ScheduleService : IScheduleService
{
    private readonly SofiaDbContext _context;

    public ScheduleService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleViewModel?> GetScheduleAsync(int psychologistUserId)
    {
        var psychologist = await _context.Psychologists
            .FirstOrDefaultAsync(p => p.UserId == psychologistUserId);

        if (psychologist == null)
            return null;

        var startDate = DateTime.Today;
        var endDate = startDate.AddDays(14);

        var schedules = await _context.PsychologistSchedules
            .Where(s => s.PsychologistId == psychologist.Id)
            .ToListAsync();

        schedules = schedules
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .ToList();

        var slots = await _context.PsychologistTimeSlots
            .Where(t => t.PsychologistId == psychologist.Id &&
                        t.Date >= startDate &&
                        t.Date <= endDate)
            .ToListAsync();

        slots = slots
            .OrderBy(t => t.Date)
            .ThenBy(t => t.StartTime)
            .ToList();

        return new ScheduleViewModel
        {
            Psychologist = psychologist,
            Schedules = schedules,
            ExistingSlots = slots,
            StartDate = startDate,
            EndDate = endDate
        };
    }

    public async Task<(bool Success, string Message)> AddScheduleAsync(int psychologistUserId, AddScheduleRequest request)
    {
        var psychologist = await _context.Psychologists
            .FirstOrDefaultAsync(p => p.UserId == psychologistUserId);

        if (psychologist == null)
            return (false, "Психолог не найден");

        var day = (DayOfWeek)request.DayOfWeek;

        // Проверка пересечений
        var exists = await _context.PsychologistSchedules
            .AnyAsync(s =>
                s.PsychologistId == psychologist.Id &&
                s.DayOfWeek == day &&
                (
                    (s.StartTime <= request.StartTime && s.EndTime > request.StartTime) ||
                    (s.StartTime < request.EndTime && s.EndTime >= request.EndTime) ||
                    (s.StartTime >= request.StartTime && s.EndTime <= request.EndTime)
                ));

        if (exists)
            return (false, "Время пересекается с существующим расписанием");

        var schedule = new PsychologistSchedule
        {
            PsychologistId = psychologist.Id,
            DayOfWeek = day,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            IsAvailable = true,
            CreatedAt = DateTime.Now
        };

        _context.PsychologistSchedules.Add(schedule);
        await _context.SaveChangesAsync();

        return (true, "Расписание добавлено");
    }

    public async Task<(bool Success, string Message)> RemoveScheduleAsync(int psychologistUserId, int scheduleId)
    {
        var psychologist = await _context.Psychologists
            .FirstOrDefaultAsync(p => p.UserId == psychologistUserId);

        if (psychologist == null)
            return (false, "Психолог не найден");

        var schedule = await _context.PsychologistSchedules
            .FirstOrDefaultAsync(s => s.Id == scheduleId && s.PsychologistId == psychologist.Id);

        if (schedule == null)
            return (false, "Расписание не найдено");

        _context.PsychologistSchedules.Remove(schedule);
        await _context.SaveChangesAsync();

        return (true, "Расписание удалено");
    }

    public async Task<(bool Success, string Message, int SlotsCreated)> AddTimeSlotsAsync(int psychologistUserId, AddTimeSlotRequest request)
    {
        var psychologist = await _context.Psychologists
            .FirstOrDefaultAsync(p => p.UserId == psychologistUserId);

        if (psychologist == null)
            return (false, "Психолог не найден", 0);

        var date = DateTime.Parse(request.Date);
        var start = TimeSpan.Parse(request.StartTime);
        var end = TimeSpan.Parse(request.EndTime);

        if (end <= start)
            return (false, "Время окончания должно быть позже начала", 0);

        var newSlots = new List<PsychologistTimeSlot>();
        var current = start;

        while (current < end)
        {
            var exists = await _context.PsychologistTimeSlots
                .AnyAsync(s =>
                    s.PsychologistId == psychologist.Id &&
                    s.Date.Date == date.Date &&
                    s.StartTime == current);

            if (!exists)
            {
                newSlots.Add(new PsychologistTimeSlot
                {
                    PsychologistId = psychologist.Id,
                    Date = date,
                    StartTime = current,
                    EndTime = current.Add(TimeSpan.FromHours(1)),
                    IsAvailable = true,
                    IsBooked = false,
                    CreatedAt = DateTime.Now
                });
            }

            current = current.Add(TimeSpan.FromHours(1));
        }

        if (newSlots.Any())
        {
            _context.PsychologistTimeSlots.AddRange(newSlots);
            await _context.SaveChangesAsync();
        }

        return (true, $"Создано {newSlots.Count} слотов", newSlots.Count);
    }
}
