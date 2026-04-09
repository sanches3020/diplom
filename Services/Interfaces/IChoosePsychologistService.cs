using Sofia.Web.ViewModel.ChoosePsychologist;

namespace Sofia.Web.Services.Interfaces;

public interface IChoosePsychologistService
{
    Task<ChoosePsychologistViewModel> GetActivePsychologistsAsync();
}
