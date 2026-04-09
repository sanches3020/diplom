using Sofia.Web.Models;
using Sofia.Web.ViewModels.Psychologist;

namespace Sofia.Web.ViewModels.Psychologists;

public class PsychologistDashboardViewModel
{
    public Sofia.Web.Models.Psychologist Psychologist { get; set; } = null!;
    public List<PsychologistAppointment> Appointments { get; set; } = new();
    public List<ClientDataViewModel> Clients { get; set; } = new();
}
