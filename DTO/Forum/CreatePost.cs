public class CreatePostDto
{
    public int ThreadId { get; set; }
    public string Content { get; set; } = null!;
    public Guid? MediaFileId { get; set; }
}