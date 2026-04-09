using Sofia.Web.ViewModels.Settings;

namespace Sofia.Web.Services.Interfaces;

public interface ISettingsService
{
    Task<SettingsIndexViewModel?> GetIndexAsync(string userId);
    Task<ProfileViewModel?> GetProfileAsync(string userId);
    Task<(bool Success, string Message)> UpdateProfileAsync(string userId, string name, string email, string bio);
    Task<(bool Success, string Message, byte[]? Bytes, string? ContentType, string? FileName)> ExportDataAsync(string userId, string format);
}
