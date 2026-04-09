using Sofia.Web.Models;

public class ForumThread
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public string AuthorId { get; set; } = null!;

    public int CategoryId { get; set; }
    public ForumCategory Category { get; set; } = null!;

    public List<ForumPost> Posts { get; set; } = new();
}
