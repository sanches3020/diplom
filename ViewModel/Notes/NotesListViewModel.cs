using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Notes;

public class NotesListViewModel
{
    public List<Note> Notes { get; set; } = new();
}
