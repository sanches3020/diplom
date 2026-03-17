using Sofia.Web.Models;
using Sofia.Web.ViewModels.Practices;

namespace Sofia.Web.Services.Interfaces;

public interface IPracticesService
{
    Task<PracticesListViewModel> GetPracticesAsync(string? category, int? duration);
    Task<Practice?> GetPracticeAsync(int id);
}
