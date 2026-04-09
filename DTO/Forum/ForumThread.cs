public class ForumThreadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int PostsCount { get; set; }
}