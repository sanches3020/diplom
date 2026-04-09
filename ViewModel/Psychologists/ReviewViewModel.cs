using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Psychologists;

public class ReviewViewModel
{
    public PsychologistReview Review { get; set; } = null!;
    public ApplicationUser? User { get; set; }
}
