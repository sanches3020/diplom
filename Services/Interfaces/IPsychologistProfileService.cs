using Sofia.Web.ViewModels.Psychologists;

namespace Sofia.Web.Services.Interfaces;

public interface IPsychologistProfileService
{
    Task<PsychologistProfileViewModel?> GetProfileAsync(int psychologistId);
}
