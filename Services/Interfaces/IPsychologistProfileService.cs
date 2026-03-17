using Sofia.Web.ViewModels.Psychologist;

namespace Sofia.Web.Services.Interfaces;

public interface IPsychologistProfileService
{
    Task<PsychologistProfileViewModel?> GetProfileAsync(int psychologistId);
}
