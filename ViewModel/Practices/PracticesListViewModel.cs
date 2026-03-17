using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Practices;

public class PracticesListViewModel
{
    public List<Practice> Practices { get; set; } = new();
    public PracticeCategory[] Categories { get; set; } = Array.Empty<PracticeCategory>();
    public string? SelectedCategory { get; set; }
    public int? SelectedDuration { get; set; }
}
