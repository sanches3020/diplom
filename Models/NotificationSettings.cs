using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofia.Web.Models
{
    public class NotificationSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Ежедневное напоминание о заметке
        public bool DailyReminder { get; set; } = false;
        public TimeSpan DailyReminderTime { get; set; } = TimeSpan.FromHours(9);

        // Напоминание о целях
        public bool GoalReminder { get; set; } = false;

        // Напоминание о проверке настроения
        public bool MoodCheckReminder { get; set; } = false;
        public TimeSpan MoodCheckTime { get; set; } = TimeSpan.FromHours(20);

        // Еженедельный отчёт
        public bool WeeklyReport { get; set; } = false;
        public int WeeklyReportDay { get; set; } = 1;

        // Напоминание о практиках
        public bool PracticeReminder { get; set; } = false;

        // Напоминание о записи к психологу
        public bool PsychologistReminder { get; set; } = false;

        // Email уведомления
        public bool EmailNotifications { get; set; } = true;

        // Push уведомления
        public bool PushNotifications { get; set; } = false;

        // Дата обновления
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
