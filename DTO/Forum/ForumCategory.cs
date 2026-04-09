public class ForumCategoryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int ThreadsCount { get; set; }
}