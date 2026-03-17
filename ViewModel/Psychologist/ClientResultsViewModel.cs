using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Psychologist;

public class ClientResultsViewModel
{
    public User Client { get; set; } = null!;
    public Psychologist Psychologist { get; set; } = null!;
    public List<TestResult> Results { get; set; } = new();
}
