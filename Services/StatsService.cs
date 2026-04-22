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
        var startDate = DateTime.UtcNow.AddDays(-daysBack);

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

            PracticeStats = await _context.Practices
                .AsNoTracking()
                .Where(p => p.IsActive)
                .ToListAsync(),

            MoodTrends = await _context.EmotionEntries
                .Where(e => e.UserId == userId &&
                            e.Date >= DateTime.UtcNow.AddDays(-7))
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

        var scopedNotes = await _context.Notes
            .AsNoTracking()
            .Where(n => n.UserId == userId && n.Date >= startDate)
            .Select(n => new { n.Date, n.CreatedAt, n.Tags, n.Activity })
            .ToListAsync();

        vm.WeeklyStats = scopedNotes
            .GroupBy(n => n.Date.DayOfWeek)
            .Select(g => new { DayOfWeek = g.Key, Count = g.Count() })
            .Cast<object>()
            .ToList();

        vm.HourlyStats = scopedNotes
            .GroupBy(n => n.CreatedAt.Hour)
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .Cast<object>()
            .ToList();

        vm.TagStats = scopedNotes
            .Where(n => !string.IsNullOrWhiteSpace(n.Tags))
            .SelectMany(n => (n.Tags ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t)))
            .GroupBy(t => t, StringComparer.OrdinalIgnoreCase)
            .Select(g => new { Tag = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(10)
            .Cast<object>()
            .ToList();

        vm.ActivityStats = scopedNotes
            .Where(n => !string.IsNullOrWhiteSpace(n.Activity))
            .GroupBy(n => n.Activity!)
            .Select(g => new { Activity = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(10)
            .Cast<object>()
            .ToList();

        return vm;
    }

    public async Task<byte[]> ExportCsvAsync(string userId, int daysBack)
    {
        var startDate = DateTime.UtcNow.AddDays(-daysBack);

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
        var last30 = DateTime.UtcNow.AddDays(-30);

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
        var start = DateTime.UtcNow.AddDays(-daysBack);
        var end = DateTime.UtcNow;

        var notes = await _context.Notes
            .AsNoTracking()
            .Where(n => n.UserId == userId && n.Date >= start && n.Date <= end)
            .ToListAsync();

        var goals = await _context.Goals
            .AsNoTracking()
            .Where(g => g.UserId == userId)
            .ToListAsync();

        var emotionStats = notes
            .GroupBy(n => n.Emotion)
            .Select(g => new EmotionStatItem { Emotion = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToList();

        var activityStats = notes
            .Where(n => !string.IsNullOrWhiteSpace(n.Activity))
            .GroupBy(n => n.Activity!)
            .Select(g => new ActivityStatItem { Activity = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToList();

        var reportData = new StatsReportData
        {
            Period = new ReportPeriod { Start = start, End = end, Days = daysBack },
            GeneratedAt = DateTime.UtcNow,
            EmotionStats = emotionStats,
            ActivityStats = activityStats,
            Goals = goals,
            Summary = new ReportSummary
            {
                TotalNotes = notes.Count,
                CompletedGoals = goals.Count(g => g.Status == GoalStatus.Completed),
                ActiveGoals = goals.Count(g => g.Status == GoalStatus.Active),
                AverageMood = notes.Count == 0 ? 0 : notes.Average(n => (int)n.Emotion),
                MostFrequentEmotion = emotionStats.FirstOrDefault()?.Emotion,
                MostFrequentActivity = activityStats.FirstOrDefault()?.Activity
            },
            GoalStats = new GoalStats
            {
                Total = goals.Count,
                Completed = goals.Count(g => g.Status == GoalStatus.Completed),
                Active = goals.Count(g => g.Status == GoalStatus.Active),
                AverageProgress = goals.Count == 0 ? 0 : goals.Average(g => g.Progress)
            }
        };

        return new ReportViewModel
        {
            ReportData = reportData,
            Format = format
        };
    }
}
