using Sofia.Web.Models;
using Sofia.Web.ViewModels.Psychologist;

<<<<<<< HEAD:ViewModel/Psychologists/PsychologistDashboardViewModel.cs
namespace Sofia.Web.ViewModels.Psychologists;
=======
namespace Sofia.Web.ViewModels.PsychologistArea;
>>>>>>> f16d9d638339ecefc9454ffc3fa28f05066aabab:ViewModel/Psychologist/PsychologistDashboardViewModel.cs

public class PsychologistDashboardViewModel
{
    public Sofia.Web.Models.Psychologist Psychologist { get; set; } = null!;
    public List<PsychologistAppointment> Appointments { get; set; } = new();
    public List<ClientDataViewModel> Clients { get; set; } = new();
}
