using Sofia.Web.ViewModels.Settings;

namespace Sofia.Web.Services.Interfaces;

public interface ISettingsService
{
    Task<SettingsIndexViewModel?> GetIndexAsync(int userId);
    Task<ProfileViewModel?> GetProfileAsync(int userId);
    Task<(bool Success, string Message)> UpdateProfileAsync(int userId, string name, string email, string bio);
    Task<(bool Success, string Message, byte[]? Bytes, string? ContentType, string? FileName)> ExportDataAsync(int userId, string format);
}
