using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Companion;

public class CompanionViewModel
{
    public string PetMood { get; set; } = "neutral";
    public string PetMessage { get; set; } = "";
    public EmotionType LastEmotion { get; set; }
    public List<Note> RecentNotes { get; set; } = new();
    public int NotesCount { get; set; }
    public int CompanionLevel { get; set; } = 1;
    public int Happiness { get; set; } = 50;
    public int Energy { get; set; } = 50;
    public int Comfort { get; set; } = 50;
}
