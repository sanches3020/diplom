using Sofia.Web.Models;

<<<<<<< HEAD:ViewModel/Psychologists/ReviewViewModel.cs
namespace Sofia.Web.ViewModels.Psychologists;
=======
namespace Sofia.Web.ViewModels.PsychologistArea;
>>>>>>> f16d9d638339ecefc9454ffc3fa28f05066aabab:ViewModel/Psychologist/ReviewViewModel.cs

public class ReviewViewModel
{
    public PsychologistReview Review { get; set; } = null!;
    public ApplicationUser? User { get; set; }
}
