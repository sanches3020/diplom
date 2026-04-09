using Sofia.Web.ViewModels.Psychologist;

namespace Sofia.Web.Services.Interfaces;

public interface IPsychologistService
{
    Task<int?> GetPsychologistIdForUserAsync(string userId);
    Task<PsychologistIndexViewModel> GetIndexDataAsync(string userId);
}
