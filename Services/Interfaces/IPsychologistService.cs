using Sofia.Web.ViewModels.PsychologistArea;

namespace Sofia.Web.Services.Interfaces;

public interface IPsychologistService
{
    Task<int?> GetPsychologistIdForUserAsync(int userId);
    Task<PsychologistIndexViewModel> GetIndexDataAsync(int userId);
}
