using Sofia.Web.Models;

public class AdminLog
{
    public int Id { get; set; }

    public string AdminId { get; set; } = null!;
    public ApplicationUser Admin { get; set; } = null!;

    public string Action { get; set; } = null!;
    public string? TargetUserId { get; set; }
    public string? Details { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
