using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Stats;
using System.Text;
using System.Text.Json;

namespace Sofia.Web.Services;

public class StatsService : IStatsService
{
    private readonly SofiaDbContext _context;

    public StatsService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<StatsIndexViewModel> GetStatsAsync(string userId, int daysBack)
    {
        var startDate = DateTime.Now.AddDays(-daysBack);

        var vm = new StatsIndexViewModel
        {
            DaysBack = daysBack,

            TotalNotes = await _context.Notes
                .CountAsync(n => n.UserId == userId),

            RecentNotes = await _context.Notes
                .CountAsync(n => n.UserId == userId && n.Date >= startDate),

            TotalGoals = await _context.Goals
                .CountAsync(g => g.UserId == userId),

            ActiveGoals = await _context.Goals
                .CountAsync(g => g.UserId == userId && g.Status == GoalStatus.Active),

            CompletedGoals = await _context.Goals
                .CountAsync(g => g.UserId == userId && g.Status == GoalStatus.Completed),

            TotalEmotions = await _context.EmotionEntries
                .CountAsync(e => e.UserId == userId),

            EmotionStats = await _context.EmotionEntries
                .Where(e => e.UserId == userId && e.Date >= startDate)
                .GroupBy(e => e.Emotion)
                .Select(g => new { Emotion = g.Key, Count = g.Count() })
                .ToListAsync<object>(),

            WeeklyStats = await _context.Notes
                .Where(n => n.UserId == userId && n.Date >= startDate)
                .GroupBy(n => n.Date.DayOfWeek)
                .Select(g => new { DayOfWeek = g.Key, Count = g.Count() })
                .ToListAsync<object>(),

            HourlyStats = await _context.Notes
                .Where(n => n.UserId == userId && n.CreatedAt >= startDate)
                .GroupBy(n => n.CreatedAt.Hour)
                .Select(g => new { Hour = g.Key, Count = g.Count() })
                .ToListAsync<object>(),

            TagStats = await _context.Notes
                .Where(n => n.UserId == userId &&
                            n.Date >= startDate &&
                            !string.IsNullOrEmpty(n.Tags))
                .SelectMany(n => n.Tags!.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .GroupBy(t => t.Trim())
                .Select(g => new { Tag = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToListAsync<object>(),

            ActivityStats = await _context.Notes
                .Where(n => n.UserId == userId &&
                            n.Date >= startDate &&
                            !string.IsNullOrEmpty(n.Activity))
                .GroupBy(n => n.Activity)
                .Select(g => new { Activity = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToListAsync<object>(),

            PracticeStats = await _context.Practices
                .AsNoTracking()
                .Where(p => p.IsActive)
                .ToListAsync(),

            MoodTrends = await _context.EmotionEntries
                .Where(e => e.UserId == userId &&
                            e.Date >= DateTime.Now.AddDays(-7))
                .GroupBy(e => e.Date.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    AverageMood = g.Average(e => (int)e.Emotion),
                    Count = g.Count()
                })
                .OrderBy(g => g.Date)
                .ToListAsync<object>()
        };

        return vm;
    }

    public async Task<byte[]> ExportCsvAsync(string userId, int daysBack)
    {
        var startDate = DateTime.Now.AddDays(-daysBack);

        var notes = await _context.Notes
            .AsNoTracking()
            .Where(n => n.UserId == userId && n.Date >= startDate)
            .OrderByDescending(n => n.Date)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Дата,Время,Эмоция,Содержание,Теги,Активность,Закреплено,Поделиться");

        foreach (var n in notes)
        {
            sb.AppendLine($"{n.Date:yyyy-MM-dd},{n.CreatedAt:HH:mm},{n.Emotion},\"{n.Content.Replace("\"", "\"\"")}\",{n.Tags},{n.Activity},{n.IsPinned},{n.ShareWithPsychologist}");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<InsightsViewModel> GetInsightsAsync(string userId)
    {
        var vm = new InsightsViewModel();
        var last30 = DateTime.Now.AddDays(-30);

        var notes = await _context.Notes
            .AsNoTracking()
            .Where(n => n.UserId == userId && n.Date >= last30)
            .ToListAsync();

        var emotions = await _context.EmotionEntries
            .AsNoTracking()
            .Where(e => e.UserId == userId && e.Date >= last30)
            .ToListAsync();

        // Здесь — твоя логика инсайтов (я переношу её 1:1)
        if (notes.Count > 5)
        {
            vm.Insights.Add(("🔁 Повторение", "Вы создаёте заметки регулярно"));
        }

        return vm;
    }

    public async Task<ReportViewModel> GenerateReportAsync(string userId, int daysBack, string format)
    {
        var start = DateTime.Now.AddDays(-daysBack);
        var end = DateTime.Now;

        var notes = await _context.Notes
            .AsNoTracking()
            .Where(n => n.UserId == userId && n.Date >= start && n.Date <= end)
            .ToListAsync();

        var goals = await _context.Goals
            .AsNoTracking()
            .Where(g => g.UserId == userId)
            .ToListAsync();

        var reportData = new
        {
            Period = new { Start = start, End = end, Days = daysBack },
            Notes = notes,
            Goals = goals,
            GeneratedAt = DateTime.Now
        };

        return new ReportViewModel
        {
            ReportData = reportData,
            Format = format
        };
    }
}
