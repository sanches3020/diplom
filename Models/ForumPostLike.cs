using Sofia.Web.Models;

public class ForumPostLike
{
    public int Id { get; set; }

    public int PostId { get; set; }
    public ForumPost Post { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
