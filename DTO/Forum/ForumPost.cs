public class ForumPostDto
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int Likes { get; set; }
    public string AuthorId { get; set; } = null!;
    public string? MediaUrl { get; set; }
}