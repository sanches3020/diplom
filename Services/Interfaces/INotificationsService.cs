using Sofia.Web.DTO.Notifications;
using Sofia.Web.Models;
using Sofia.Web.ViewModels.Notifications;

namespace Sofia.Web.Services.Interfaces;

public interface INotificationsService
{
    Task<NotificationsViewModel> GetNotificationsAsync();
    Task<bool> MarkAsReadAsync(int id);
    Task<bool> MarkAllAsReadAsync();
    Task<bool> DismissAsync(int id);

    Task<NotificationSettings> GetSettingsAsync();
    Task<bool> UpdateSettingsAsync(UpdateNotificationSettingsRequest request);

    Task<int> CreateNotificationAsync(CreateNotificationRequest request);

    Task<List<NotificationResponse>> CheckNotificationsAsync();
}
