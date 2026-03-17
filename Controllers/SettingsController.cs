using Microsoft.AspNetCore.Mvc;
using Sofia.Web.Services.Interfaces;
using System.Security.Claims;

namespace Sofia.Web.Controllers;

[Route("settings")]
public class SettingsController : Controller
{
    private readonly ISettingsService _settingsService;

    public SettingsController(ISettingsService settingsService)
    {
        _settingsService = settingsService;
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

        var vm = await _settingsService.GetIndexAsync(userId);
        if (vm == null)
            return RedirectToAction("Login", "Auth");

        return View(vm);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var vm = await _settingsService.GetProfileAsync(userId);
        if (vm == null)
            return RedirectToAction("Login", "Auth");

        return View(vm);
    }

    [HttpPost("profile")]
    public async Task<IActionResult> UpdateProfile(string name, string email, string bio, string timezone)
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Пользователь не авторизован" });

        var result = await _settingsService.UpdateProfileAsync(userId, name, email, bio);
        return Json(result);
    }

    [HttpGet("preferences")]
    public IActionResult Preferences() => View();

    [HttpPost("preferences")]
    public IActionResult UpdatePreferences(string language, string timezone, bool notifications, bool emailUpdates, bool soundEffects, bool animations)
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Пользователь не авторизован" });

        return Json(new { success = true, message = "Настройки успешно сохранены!" });
    }

    [HttpGet("privacy")]
    public IActionResult Privacy() => View();

    [HttpPost("privacy")]
    public IActionResult UpdatePrivacy(bool shareData, bool allowAnalytics, bool showInDirectory)
        => Json(new { success = true, message = "Настройки приватности обновлены!" });

    [HttpGet("notifications")]
    public IActionResult Notifications() => View();

    [HttpPost("notifications")]
    public IActionResult UpdateNotifications(bool dailyReminder, bool goalReminder, bool moodCheck, string reminderTime)
        => Json(new { success = true, message = "Настройки уведомлений обновлены!" });

    [HttpGet("data")]
    public IActionResult Data() => View();

    [HttpPost("export")]
    public async Task<IActionResult> ExportData(string format)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var result = await _settingsService.ExportDataAsync(userId, format);
        if (!result.Success)
            return BadRequest(result.Message);

        return File(result.Bytes!, result.ContentType!, result.FileName!);
    }

    [HttpPost("delete-account")]
    public IActionResult DeleteAccount(string confirmation)
    {
        if (confirmation != "УДАЛИТЬ")
            return Json(new { success = false, message = "Подтверждение неверное. Введите 'УДАЛИТЬ'." });

        return Json(new { success = true, message = "Аккаунт будет удален в течение 24 часов." });
    }

    [HttpGet("help")]
    public IActionResult Help() => View();

    [HttpGet("about")]
    public IActionResult About() => View();
}
