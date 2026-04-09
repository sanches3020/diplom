using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Settings;

public class ProfileViewModel
{
    public ApplicationUser User { get; set; } = null!;
    public int TotalNotes { get; set; }
    public int TotalGoals { get; set; }
    public int CompletedGoals { get; set; }
    public int TotalEmotions { get; set; }
}
