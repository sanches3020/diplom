using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.DTO.Calendar;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Calendar;

namespace Sofia.Web.Services;

public class CalendarService : ICalendarService
{
    private readonly SofiaDbContext _context;

    public CalendarService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<CalendarIndexViewModel> GetCalendarAsync(string userId, int? year, int? month)
    {
        var targetDate = DateTime.Now;

        if (year.HasValue && month.HasValue)
            targetDate = new DateTime(year.Value, month.Value, 1);

        var firstDayOfMonth = new DateTime(targetDate.Year, targetDate.Month, 1);

        var startDate = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek + 1);
        if (firstDayOfMonth.DayOfWeek == DayOfWeek.Sunday)
            startDate = startDate.AddDays(-6);

        var endDate = startDate.AddDays(41);

        var notes = await _context.Notes
            .Where(n => n.UserId == userId &&
                        n.Date.Date >= startDate.Date &&
                        n.Date.Date < endDate.Date)
            .ToListAsync();

        var emotions = await _context.EmotionEntries
            .Where(e => e.UserId == userId &&
                        e.Date.Date >= startDate.Date &&
                        e.Date.Date < endDate.Date)
            .ToListAsync();

        var days = new List<CalendarDayViewModel>();

        for (var date = startDate.Date; date < endDate.Date; date = date.AddDays(1))
        {
            var dateKey = date.Date;

            days.Add(new CalendarDayViewModel
            {
                Date = dateKey,
                Notes = notes.Where(n => n.Date.Date == dateKey).ToList(),
                Emotions = emotions.Where(e => e.Date.Date == dateKey).ToList()
            });
        }

        return new CalendarIndexViewModel
        {
            CurrentMonth = targetDate,
            PreviousMonth = targetDate.AddMonths(-1),
            NextMonth = targetDate.AddMonths(1),
            Days = days
        };
    }

    public async Task<(bool Success, string Message)> SaveEmotionAsync(string userId, DTO.Calendar.SaveEmotionRequest request)
    {
        if (string.IsNullOrEmpty(request.Date))
            return (false, "Дата не указана");

        if (!DateTime.TryParse(request.Date, out var date))
            return (false, "Неверный формат даты");

        date = date.Date;

        if (!Enum.TryParse<EmotionType>(request.Emotion, true, out var emotionType))
            return (false, "Неверный тип эмоции: " + request.Emotion);

        var existingEmotions = await _context.EmotionEntries
            .CountAsync(e => e.UserId == userId && e.Date.Date == date.Date);

        if (existingEmotions >= 5)
            return (false, "Максимум 5 эмоций в день");

        var emotionEntry = new EmotionEntry
        {
            UserId = userId,
            Date = date,
            Emotion = emotionType,
            Note = request.Note,
            CreatedAt = DateTime.Now
        };

        _context.EmotionEntries.Add(emotionEntry);
        await _context.SaveChangesAsync();

        return (true, "Эмоция сохранена!");
    }

    public async Task<DayDetailsViewModel?> GetDayDetailsAsync(string userId, DateTime date)
    {
        var targetDate = date.Date;

        var notes = await _context.Notes
            .Where(n => n.UserId == userId && n.Date.Date == targetDate)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        var emotions = await _context.EmotionEntries
            .Where(e => e.UserId == userId && e.Date.Date == targetDate)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        var goals = await _context.Goals
            .Where(g => g.UserId == userId && g.Date.Date == targetDate)
            .ToListAsync();

        return new DayDetailsViewModel
        {
            Date = targetDate,
            Notes = notes,
            Emotions = emotions,
            Goals = goals
        };
    }

public async Task<(IEnumerable<object> EmotionStats, int DaysBack)> GetEmotionStatsAsync(string userId, int? days)
{
    var daysBack = days ?? 30;
    var startDate = DateTime.Now.AddDays(-daysBack);

    var emotionStats = await _context.EmotionEntries
        .Where(e => e.UserId == userId && e.CreatedAt >= startDate)
        .GroupBy(e => e.Emotion)
        .Select(g => new
        {
            Emotion = g.Key.ToString(),
            Count = g.Count()
        })
        .ToListAsync();

    return (emotionStats, daysBack);
}

}
