using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Notifications;

public class NotificationsViewModel
{
    public List<Notification> Notifications { get; set; } = new();
    public int UnreadCount { get; set; }
}
