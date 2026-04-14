using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.PsychologistArea;

public class PsychologistIndexViewModel
{
    public List<Sofia.Web.Models.Psychologist> Psychologists { get; set; } = [];
    public List<Note> RecentNotes { get; set; } = new();
}
