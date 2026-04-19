using Sofia.Web.Models;

<<<<<<< HEAD:ViewModel/Psychologists/PsychologistProfileViewModel.cs
namespace Sofia.Web.ViewModels.Psychologists;
=======
namespace Sofia.Web.ViewModels.PsychologistArea;
>>>>>>> f16d9d638339ecefc9454ffc3fa28f05066aabab:ViewModel/Psychologist/PsychologistProfileViewModel.cs

public class PsychologistProfileViewModel
{
    public Sofia.Web.Models.Psychologist Psychologist { get; set; } = null!;
    public List<ReviewViewModel> Reviews { get; set; } = new();
    public List<PsychologistTimeSlot> AvailableSlots { get; set; } = new();
}
