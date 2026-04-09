using Sofia.Web.ViewModels.Home;

namespace Sofia.Web.Services.Interfaces;

public interface IHomeService
{
    Task<int?> GetPsychologistIdForUserAsync(string userId);
    Task<HomeIndexViewModel> GetHomePageDataAsync();
}
