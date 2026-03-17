using Sofia.Web.DTO.Companion;
using Sofia.Web.ViewModels.Companion;

namespace Sofia.Web.Services.Interfaces;

public interface ICompanionService
{
    Task<CompanionViewModel> GetCompanionDataAsync(int userId);
    Task<CompanionStatusResponse> GetStatusAsync(int userId);

    (int Value, string Message, string Mood) Feed();
    (int Value, string Message, string Mood) Play();
    (int Value, string Message, string Mood) Comfort();
}
