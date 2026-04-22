using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Stats;

public class ReportViewModel
{
    public StatsReportData ReportData { get; set; } = new();
    public string Format { get; set; } = "html";
}

public class StatsReportData
{
    public ReportPeriod Period { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public ReportSummary Summary { get; set; } = new();
    public List<EmotionStatItem> EmotionStats { get; set; } = new();
    public List<ActivityStatItem> ActivityStats { get; set; } = new();
    public List<Goal> Goals { get; set; } = new();
    public GoalStats GoalStats { get; set; } = new();
}

public class ReportPeriod
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int Days { get; set; }
}

public class ReportSummary
{
    public int TotalNotes { get; set; }
    public int CompletedGoals { get; set; }
    public int ActiveGoals { get; set; }
    public double AverageMood { get; set; }
    public EmotionType? MostFrequentEmotion { get; set; }
    public string? MostFrequentActivity { get; set; }
}

public class EmotionStatItem
{
    public EmotionType Emotion { get; set; }
    public int Count { get; set; }
}

public class ActivityStatItem
{
    public string Activity { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class GoalStats
{
    public int Total { get; set; }
    public int Completed { get; set; }
    public int Active { get; set; }
    public double AverageProgress { get; set; }
}
