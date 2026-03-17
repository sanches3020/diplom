using Sofia.Web.Controllers;
using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.PsychologistArea;

public class PsychologistDashboardViewModel
{
    public Psychologist Psychologist { get; set; } = null!;
    public List<PsychologistAppointment> Appointments { get; set; } = new();
    public List<ClientDataViewModel> Clients { get; set; } = new();
}
