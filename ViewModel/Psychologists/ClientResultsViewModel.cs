using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.PsychologistArea;

public class ClientResultsViewModel
{
    public ApplicationUser Client { get; set; } = null!;
<<<<<<< HEAD:ViewModel/Psychologists/ClientResultsViewModel.cs
    public Sofia.Web.Models.Psychologist Psychologist { get; set; }

=======
    public Psychologist Psychologist { get; set; } = null!;
>>>>>>> f16d9d638339ecefc9454ffc3fa28f05066aabab:ViewModel/Psychologist/ClientResultsViewModel.cs
    public List<TestResult> Results { get; set; } = new();
}
