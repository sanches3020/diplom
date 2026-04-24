using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.DTO.Psychologist;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModel.Psychologists;

namespace Sofia.Web.Services;

public class AppointmentsService : IAppointmentsService
{
    private readonly SofiaDbContext _context;

    public AppointmentsService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<BookAppointmentResult> BookAppointmentAsync(string userId, BookAppointmentRequest request)
    {
        // Парсим дату
        if (!TryParseDate(request.AppointmentDate, out var appointmentDateLocal))
        {
            return new BookAppointmentResult
            {
                Success = false,
                Message = "Неверный формат даты"
            };
        }

        var dateOnly = appointmentDateLocal.Date;
        var time = appointmentDateLocal.TimeOfDay;

        // Ищем слоты на этот день
        var slots = await _context.PsychologistTimeSlots
            .Where(t => t.PsychologistId == request.PsychologistId &&
                        t.Date.Date == dateOnly &&
                        t.IsAvailable && !t.IsBooked)
            .ToListAsync();

        // Ищем точное совпадение
        var matchedSlot = slots.FirstOrDefault(s =>
            s.StartTime.Hours == time.Hours &&
            s.StartTime.Minutes == time.Minutes);

        PsychologistTimeSlot? selectedSlot = matchedSlot;

        // Если нет точного — ищем ближайший
        if (selectedSlot == null)
        {
            if (slots.Any())
            {
                selectedSlot = slots
                    .OrderBy(s => Math.Abs((s.StartTime - time).TotalMinutes))
                    .First();
            }
            else
            {
                // Ищем ближайший в будущем
                selectedSlot = await _context.PsychologistTimeSlots
                    .Where(t => t.PsychologistId == request.PsychologistId &&
                                t.Date.Date > dateOnly &&
                                t.IsAvailable && !t.IsBooked)
                    .OrderBy(t => t.Date)
                    .ThenBy(t => t.StartTime)
                    .FirstOrDefaultAsync();
            }
        }

        if (selectedSlot == null)
        {
            return new BookAppointmentResult
            {
                Success = false,
                Message = "Нет доступных слотов на выбранную дату и ближайшие дни"
            };
        }

        // Бронируем слот
        selectedSlot.IsBooked = true;
        selectedSlot.BookedByUserId = userId;

        var appointmentDateTime = selectedSlot.Date.Date.Add(selectedSlot.StartTime);

        var appointment = new PsychologistAppointment
        {
            PsychologistId = request.PsychologistId,
            UserId = userId,
            AppointmentDate = appointmentDateTime,
            Notes = request.Notes ?? "",
            Status = AppointmentStatus.Scheduled,
            CreatedAt = DateTime.UtcNow
        };

        _context.PsychologistAppointments.Add(appointment);
        await _context.SaveChangesAsync();

        return new BookAppointmentResult
        {
            Success = true,
            Message = "Запись успешно создана!",
            AppointmentId = appointment.Id,
            AppointmentDate = appointment.AppointmentDate
        };
    }

    public async Task<UserAppointmentsViewModel> GetUserAppointmentsAsync(string userId)
    {
        var appointments = await _context.PsychologistAppointments
            .Where(a => a.UserId == userId)
            .Include(a => a.Psychologist)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        return new UserAppointmentsViewModel
        {
            Appointments = appointments
        };
    }

    private bool TryParseDate(string input, out DateTime result)
    {
        if (DateTimeOffset.TryParse(input, out var dto))
        {
            result = dto.ToLocalTime().DateTime;
            return true;
        }

        if (DateTime.TryParse(input, out var dt))
        {
            result = DateTime.SpecifyKind(dt, DateTimeKind.Local);
            return true;
        }

        result = default;
        return false;
    }
}
