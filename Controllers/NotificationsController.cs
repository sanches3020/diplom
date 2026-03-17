using Microsoft.AspNetCore.Mvc;
using Sofia.Web.DTO.Notifications;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using System.Security.Claims;

namespace Sofia.Web.Controllers;

[Route("notifications")]
public class NotificationsController : Controller
{
    private readonly INotificationsService _service;

    public NotificationsController(INotificationsService service)
    {
        _service = service;
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var vm = await _service.GetNotificationsAsync(userId);
        return View(vm);
    }

    [HttpPost("mark-read/{id}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false });

        await _service.MarkAsReadAsync(userId, id);
        return Json(new { success = true });
    }

    [HttpPost("mark-all-read")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false });

        await _service.MarkAllAsReadAsync(userId);
        return Json(new { success = true });
    }

    [HttpPost("dismiss/{id}")]
    public async Task<IActionResult> Dismiss(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false });

        await _service.DismissAsync(userId, id);
        return Json(new { success = true });
    }

    [HttpGet("settings")]
    public async Task<IActionResult> Settings()
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var settings = await _service.GetSettingsAsync(userId);
        return View(settings);
    }

    [HttpPost("settings")]
    public async Task<IActionResult> UpdateSettings(UpdateNotificationSettingsRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false });

        await _service.UpdateSettingsAsync(userId, request);
        return Json(new { success = true, message = "Настройки обновлены!" });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateNotification(CreateNotificationRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false });

        var id = await _service.CreateNotificationAsync(userId, request);
        return Json(new { success = true, notificationId = id });
    }

    [HttpGet("check")]
    public async Task<IActionResult> CheckNotifications()
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { notifications = Array.Empty<object>() });

        var list = await _service.CheckNotificationsAsync(userId);
        return Json(new { notifications = list });
    }

    [HttpPost("test")]
    public async Task<IActionResult> SendTestNotification()
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false });

        var id = await _service.CreateNotificationAsync(userId, new CreateNotificationRequest
        {
            Title = "🧪 Тестовое уведомление",
            Message = "Это тестовое уведомление.",
            Type = NotificationType.System,
            Priority = NotificationPriority.Medium
        });

        return Json(new { success = true, notificationId = id });
    }
}
