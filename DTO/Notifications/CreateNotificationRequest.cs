using Sofia.Web.Models;

namespace Sofia.Web.DTO.Notifications;

public class CreateNotificationRequest
{
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;
    public DateTime? ScheduledAt { get; set; }
}
