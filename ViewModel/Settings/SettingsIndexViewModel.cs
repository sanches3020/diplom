using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Settings;

public class SettingsIndexViewModel
{
    public ApplicationUser User { get; set; } = null!;
    public int TotalNotes { get; set; }
    public int TotalGoals { get; set; }
    public int CompletedGoals { get; set; }
    public int SharedNotes { get; set; }
    public int PinnedNotes { get; set; }
    public int TotalEmotions { get; set; }
    public List<Note> RecentNotes { get; set; } = new();
    public List<Goal> RecentGoals { get; set; } = new();
}
