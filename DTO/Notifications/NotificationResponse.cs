namespace Sofia.Web.DTO.Notifications;

public class NotificationResponse
{
    public string Type { get; set; } = "";
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public string Priority { get; set; } = "";
    public string ActionUrl { get; set; } = "";
    public string ActionText { get; set; } = "";
}
