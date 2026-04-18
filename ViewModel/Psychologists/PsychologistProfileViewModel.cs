using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Psychologists;

public class PsychologistProfileViewModel
{
    public Sofia.Web.Models.Psychologist Psychologist { get; set; } = null!;
    public List<ReviewViewModel> Reviews { get; set; } = new();
    public List<PsychologistTimeSlot> AvailableSlots { get; set; } = new();
}
