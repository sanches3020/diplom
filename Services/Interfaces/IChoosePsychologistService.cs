using Sofia.Web.ViewModels.ChoosePsychologist;

namespace Sofia.Web.Services.Interfaces;

public interface IChoosePsychologistService
{
    Task<ChoosePsychologistViewModel> GetActivePsychologistsAsync();
}
