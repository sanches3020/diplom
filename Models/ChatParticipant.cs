using Sofia.Web.Models;

public class ChatParticipant
{
    public int Id { get; set; }

    public int RoomId { get; set; }
    public ChatRoom Room { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
