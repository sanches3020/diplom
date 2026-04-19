using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.DTO.Notifications;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Notifications;

namespace Sofia.Web.Services;

public class NotificationsService : INotificationsService
{
    private readonly SofiaDbContext _context;

    public NotificationsService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationsViewModel> GetNotificationsAsync()
    {
        var notifications = await _context.Notifications
            .AsNoTracking()
            .Where(n => n.IsActive)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToListAsync();

        var unreadCount = await _context.Notifications
            .CountAsync(n => n.IsActive && !n.IsRead);

        return new NotificationsViewModel
        {
            Notifications = notifications,
            UnreadCount = unreadCount
        };
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null) return false;

        notification.IsRead = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAllAsReadAsync()
    {
        var notifications = await _context.Notifications
            .Where(n => n.IsActive && !n.IsRead)
            .ToListAsync();

        foreach (var n in notifications)
            n.IsRead = true;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DismissAsync(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null) return false;

        notification.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<NotificationSettings> GetSettingsAsync()
    {
        var settings = await _context.NotificationSettings.FirstOrDefaultAsync();
        if (settings != null) return settings;

        settings = new NotificationSettings();
        _context.NotificationSettings.Add(settings);
        await _context.SaveChangesAsync();
        return settings;
    }

    public async Task<bool> UpdateSettingsAsync(UpdateNotificationSettingsRequest request)
    {
        var settings = await GetSettingsAsync();

        settings.DailyReminder = request.DailyReminder;
        settings.DailyReminderTime = TimeSpan.Parse(request.DailyReminderTime);
        settings.GoalReminder = request.GoalReminder;
        settings.MoodCheckReminder = request.MoodCheckReminder;
        settings.MoodCheckTime = TimeSpan.Parse(request.MoodCheckTime);
        settings.WeeklyReport = request.WeeklyReport;
        settings.WeeklyReportDay = Enum.Parse<DayOfWeek>(request.WeeklyReportDay);
        settings.PracticeReminder = request.PracticeReminder;
        settings.PsychologistReminder = request.PsychologistReminder;
        settings.EmailNotifications = request.EmailNotifications;
        settings.PushNotifications = request.PushNotifications;
        settings.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> CreateNotificationAsync(CreateNotificationRequest request)
    {
        var notification = new Notification
        {
            Title = request.Title,
            Message = request.Message,
            Type = request.Type,
            Priority = request.Priority,
            ScheduledAt = request.ScheduledAt,
            CreatedAt = DateTime.Now,
            IsActive = true,
            IsRead = false
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return notification.Id;
    }

    public async Task<List<NotificationResponse>> CheckNotificationsAsync(string userId)
    {
        var now = DateTime.Now;
        var settings = await GetSettingsAsync();

        var result = new List<NotificationResponse>();

        // DAILY REMINDER
        if (settings.DailyReminder &&
            IsTimeMatch(now.TimeOfDay, settings.DailyReminderTime))
        {
            var lastNote = await _context.Notes
                .Where(n => n.UserId == userId && n.CreatedAt.Date == now.Date)
                .FirstOrDefaultAsync();

            if (lastNote == null)
            {
                result.Add(new NotificationResponse
                {
                    Type = "daily_reminder",
                    Title = "📝 Время для записи!",
                    Message = "Как прошел ваш день?",
                    Priority = "medium",
                    ActionUrl = "/notes/create",
                    ActionText = "Создать заметку"
                });
            }
        }

        // MOOD CHECK
        if (settings.MoodCheckReminder &&
            IsTimeMatch(now.TimeOfDay, settings.MoodCheckTime))
        {
            var lastMood = await _context.Notes
                .Where(n => n.UserId == userId && n.CreatedAt.Date == now.Date)
                .FirstOrDefaultAsync();

            if (lastMood == null)
            {
                result.Add(new NotificationResponse
                {
                    Type = "mood_check",
                    Title = "😊 Как ваше настроение?",
                    Message = "Проверьте своё эмоциональное состояние.",
                    Priority = "high",
                    ActionUrl = "/notes/create",
                    ActionText = "Записать настроение"
                });
            }
        }

        // GOAL REMINDER
        if (settings.GoalReminder)
        {
            var goals = await _context.Goals
                .Where(g => g.UserId == userId &&
                            g.Status == GoalStatus.Active &&
                            g.Progress < 100)
                .ToListAsync();

            foreach (var goal in goals)
            {
                var lastUpdate = await _context.Notes
                    .Where(n => n.UserId == userId &&
                                n.CreatedAt >= goal.CreatedAt &&
                                n.Content.Contains(goal.Title))
                    .OrderByDescending(n => n.CreatedAt)
                    .FirstOrDefaultAsync();

                if (lastUpdate == null || lastUpdate.CreatedAt < now.AddDays(-3))
                {
                    result.Add(new NotificationResponse
                    {
                        Type = "goal_reminder",
                        Title = $"🎯 Цель: {goal.Title}",
                        Message = $"Прогресс: {goal.Progress}%",
                        Priority = "medium",
                        ActionUrl = "/goals",
                        ActionText = "Посмотреть цели"
                    });
                    break;
                }
            }
        }

        // PRACTICE REMINDER
        if (settings.PracticeReminder)
        {
            var lastPractice = await _context.Notes
                .Where(n => n.UserId == userId &&
                            n.CreatedAt >= now.AddDays(-2) &&
                            !string.IsNullOrEmpty(n.Activity))
                .OrderByDescending(n => n.CreatedAt)
                .FirstOrDefaultAsync();

            if (lastPractice == null)
            {
                result.Add(new NotificationResponse
                {
                    Type = "practice_reminder",
                    Title = "🧘 Время для практики!",
                    Message = "Попробуйте технику релаксации.",
                    Priority = "low",
                    ActionUrl = "/practices",
                    ActionText = "Выбрать практику"
                });
            }
        }

        return result;
    }

    private bool IsTimeMatch(TimeSpan now, TimeSpan target)
    {
        return now >= target.Add(TimeSpan.FromMinutes(-5)) &&
               now <= target.Add(TimeSpan.FromMinutes(5));
    }

    public Task<NotificationsViewModel> GetNotificationsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> MarkAsReadAsync(string userId, int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> MarkAllAsReadAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DismissAsync(string userId, int id)
    {
        throw new NotImplementedException();
    }

    public Task<NotificationSettings> GetSettingsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateSettingsAsync(string userId, UpdateNotificationSettingsRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<int> CreateNotificationAsync(string userId, CreateNotificationRequest request)
    {
        throw new NotImplementedException();
    }
}
