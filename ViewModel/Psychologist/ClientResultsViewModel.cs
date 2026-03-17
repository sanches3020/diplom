using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.PsychologistArea;

public class ClientResultsViewModel
{
    public ApplicationUser Client { get; set; } = null!;
    public Psychologist Psychologist { get; set; } = null!;
    public List<TestResult> Results { get; set; } = new();
}
