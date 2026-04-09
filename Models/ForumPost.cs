namespace Sofia.Web.Models;

public class ForumPost
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public string AuthorId { get; set; } = null!;
    public ApplicationUser? Author { get; set; }

    public int ThreadId { get; set; }
    public ForumThread Thread { get; set; } = null!;

    public List<ForumPostLike> Likes { get; set; } = new();

    public Guid? MediaFileId { get; set; }
    public MediaFile? MediaFile { get; set; }
}
