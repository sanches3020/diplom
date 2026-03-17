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

        public bool DailyReminder { get; set; } = false;
        public TimeSpan DailyReminderTime { get; set; } = TimeSpan.FromHours(9);

        public bool GoalReminder { get; set; } = false;
        public bool MoodCheckReminder { get; set; } = false;
        public TimeSpan MoodCheckTime { get; set; } = TimeSpan.FromHours(20);

        public bool WeeklyReport { get; set; } = false;
        public int WeeklyReportDay { get; set; } = 1;

        public bool PracticeReminder { get; set; } = false;
        public bool PsychologistReminder { get; set; } = false;

        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = false;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
