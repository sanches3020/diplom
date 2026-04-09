public class ForumCategory
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public List<ForumThread> Threads { get; set; } = new();
}
