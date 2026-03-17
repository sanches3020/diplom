using Sofia.Web.Models;

namespace Sofia.Web.DTO.Goals;

public class UpdateGoalRequest
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public GoalType Type { get; set; }
    public GoalStatus Status { get; set; }
    public int Progress { get; set; }
    public DateTime Date { get; set; }
    public DateTime? TargetDate { get; set; }
}
