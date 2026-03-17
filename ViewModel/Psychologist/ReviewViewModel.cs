using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.PsychologistArea;

public class ReviewViewModel
{
    public PsychologistReview Review { get; set; } = null!;
    public ApplicationUser? User { get; set; }
}
