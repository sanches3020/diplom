using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Stats;

public class StatsIndexViewModel
{
    public int DaysBack { get; set; }

    public int TotalNotes { get; set; }
    public int RecentNotes { get; set; }
    public int TotalGoals { get; set; }
    public int ActiveGoals { get; set; }
    public int CompletedGoals { get; set; }
    public int TotalEmotions { get; set; }

    public List<object> EmotionStats { get; set; } = new();
    public List<object> WeeklyStats { get; set; } = new();
    public List<object> HourlyStats { get; set; } = new();
    public List<object> TagStats { get; set; } = new();
    public List<object> ActivityStats { get; set; } = new();
    public List<Practice> PracticeStats { get; set; } = new();
    public List<object> MoodTrends { get; set; } = new();
}
