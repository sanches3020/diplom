using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Calendar;

public class DayDetailsViewModel
{
    public DateTime Date { get; set; }
    public List<Note> Notes { get; set; } = new();
    public List<EmotionEntry> Emotions { get; set; } = new();
    public List<Goal> Goals { get; set; } = new();
}
