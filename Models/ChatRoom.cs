using Sofia.Web.Models;

public class ChatRoom
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public bool IsPrivate { get; set; } = false;

    public List<ChatParticipant> Participants { get; set; } = new();
    public List<ChatMessage> Messages { get; set; } = new();
}
