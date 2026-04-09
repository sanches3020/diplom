namespace Sofia.Web.Models
{
    public enum NotificationType
    {
        Info = 0,
        Warning = 1,
        Success = 2,
        Error = 3,
        System = 4,

        // --- ТИПЫ, КОТОРЫЕ ИСПОЛЬЗУЕТ ТВОЙ КОД ---
        Achievement = 10,
        DailyReminder = 11,
        GoalReminder = 12,
        MoodCheck = 13,
        PracticeReminder = 14,
        WeeklyReport = 15,
        PsychologistReminder = 16
    }
}
