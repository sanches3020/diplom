namespace Sofia.Web.Models;

public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Room { get; set; } = "general";

    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string AvatarCode { get; set; } = null!; // анонимный аватар

    public string Text { get; set; } = null!;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public bool IsSystem { get; set; } // для сообщений о входе/выходе
}
