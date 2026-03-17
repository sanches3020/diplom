using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Goals;

public class GoalsListViewModel
{
    public List<Goal> Goals { get; set; } = new();
    public string? CurrentSort { get; set; }
}
