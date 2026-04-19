using Sofia.Web.ViewModels.PsychologistArea;

namespace Sofia.Web.Services.Interfaces;

public interface IPsychologistProfileService
{
    Task<PsychologistProfileViewModel?> GetProfileAsync(int psychologistId);
}
