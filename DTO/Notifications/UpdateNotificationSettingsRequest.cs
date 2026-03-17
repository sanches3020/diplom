namespace Sofia.Web.DTO.Notifications;

public class UpdateNotificationSettingsRequest
{
    public bool DailyReminder { get; set; }
    public string DailyReminderTime { get; set; } = "";
    public bool GoalReminder { get; set; }
    public bool MoodCheckReminder { get; set; }
    public string MoodCheckTime { get; set; } = "";
    public bool WeeklyReport { get; set; }
    public string WeeklyReportDay { get; set; } = "";
    public bool PracticeReminder { get; set; }
    public bool PsychologistReminder { get; set; }
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
}
