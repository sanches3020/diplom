using Sofia.Web.DTO.Companion;
using Sofia.Web.ViewModels.Companion;

namespace Sofia.Web.Services.Interfaces;

public interface ICompanionService
{
    Task<CompanionViewModel> GetCompanionDataAsync(string userId);
    Task<CompanionStatusResponse> GetStatusAsync(string userId);

    Task<(int Value, string Message, string Mood)> FeedAsync(string userId);
    Task<(int Value, string Message, string Mood)> PlayAsync(string userId);
    Task<(int Value, string Message, string Mood)> ComfortAsync(string userId);

    // Companion Level System
    Task<CompanionLevelInfoResponse> GetCompanionLevelInfoAsync(string userId);
    Task<bool> InitializeCompanionJoinDateAsync(string userId);
}
