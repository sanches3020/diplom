using Sofia.Web.Models;

namespace Sofia.Web.ViewModels.PsychologistArea;

public class PsychologistReviewsViewModel
{
    public Psychologist Psychologist { get; set; } = null!;
    public List<PsychologistReview> Reviews { get; set; } = new();
}
