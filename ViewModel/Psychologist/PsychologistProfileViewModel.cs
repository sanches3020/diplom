using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Psychologist;

public class PsychologistProfileViewModel
{
    public Psychologist Psychologist { get; set; } = null!;
    public List<ReviewViewModel> Reviews { get; set; } = new();
    public List<PsychologistTimeSlot> AvailableSlots { get; set; } = new();
}
