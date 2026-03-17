using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Companion;

public class CompanionViewModel
{
    public string PetMood { get; set; } = "neutral";
    public string PetMessage { get; set; } = "";
    public EmotionType LastEmotion { get; set; }
    public List<Note> RecentNotes { get; set; } = new();
    public int NotesCount { get; set; }
}
