using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Psychologist;

public class PsychologistIndexViewModel
{
    public List<Sofia.Web.Models.Psychologist> Psychologists { get; set; } = [];
    public List<Note> RecentNotes { get; set; } = new();
}
