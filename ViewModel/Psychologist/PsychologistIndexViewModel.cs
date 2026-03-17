using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Psychologist;

public class PsychologistIndexViewModel
{
    public List<Psychologist> Psychologists { get; set; } = new();
    public List<Note> RecentNotes { get; set; } = new();
}
