using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Psychologist;

public class ClientResultsViewModel
{
    public ApplicationUser Client { get; set; } = null!;
    public Sofia.Web.Models.Psychologist Psychologist { get; set; }

    public List<TestResult> Results { get; set; } = new();
}
