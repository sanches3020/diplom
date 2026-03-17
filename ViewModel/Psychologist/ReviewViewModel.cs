using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Psychologist;

public class ReviewViewModel
{
    public PsychologistReview Review { get; set; } = null!;
    public User? User { get; set; }
}
