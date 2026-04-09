using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.Psychologist;

public class PsychologistReviewsViewModel
{
    public Sofia.Web.Models.Psychologist Psychologist { get; set; } = null!;
    public List<PsychologistReview> Reviews { get; set; } = new();
}
