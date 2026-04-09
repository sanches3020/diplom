namespace Sofia.Web.Models
{
    public class NotificationSettings
    {
        public int Id { get; set; }

        // Ежедневное напоминание о заметке
        public bool DailyReminder { get; set; } = false;
        public TimeSpan DailyReminderTime { get; set; } = TimeSpan.FromHours(20);

        // Напоминание о проверке настроения
        public bool MoodCheckReminder { get; set; } = false;
        public TimeSpan MoodCheckTime { get; set; } = TimeSpan.FromHours(18);

        // Напоминание о целях
        public bool GoalReminder { get; set; } = false;

        // Еженедельный отчёт
        public bool WeeklyReport { get; set; } = false;
        public DayOfWeek WeeklyReportDay { get; set; } = DayOfWeek.Sunday;

        // Напоминание о практиках
        public bool PracticeReminder { get; set; } = false;

        // Напоминание о записи к психологу
        public bool PsychologistReminder { get; set; } = false;

        // Email уведомления
        public bool EmailNotifications { get; set; } = false;

        // Push уведомления
        public bool PushNotifications { get; set; } = false;

        // Дата обновления
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
