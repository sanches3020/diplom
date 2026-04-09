using Sofia.Web.DTO.Notifications;
using Sofia.Web.Models;
using Sofia.Web.ViewModels.Notifications;

namespace Sofia.Web.Services.Interfaces;

public interface INotificationsService
{
    Task<NotificationsViewModel> GetNotificationsAsync(string userId);
    Task<bool> MarkAsReadAsync(string userId, int id);
    Task<bool> MarkAllAsReadAsync(string userId);
    Task<bool> DismissAsync(string userId, int id);

    Task<NotificationSettings> GetSettingsAsync(string userId);
    Task<bool> UpdateSettingsAsync(string userId, UpdateNotificationSettingsRequest request);

    Task<int> CreateNotificationAsync(string userId, CreateNotificationRequest request);

    Task<List<NotificationResponse>> CheckNotificationsAsync(string userId);
}
