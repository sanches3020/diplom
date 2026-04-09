public class ForumThreadFullDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public List<ForumPostDto> Posts { get; set; } = new();
}