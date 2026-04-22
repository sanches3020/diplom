using Sofia.Web.Models;

namespace Sofia.Web.ViewModel.Psychologists;

public class UserAppointmentsViewModel
{
    public List<PsychologistAppointment> Appointments { get; set; } = new();
}