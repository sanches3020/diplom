using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofia.Web.DTO.Calendar;
using Sofia.Web.Services.Interfaces;
using System.Security.Claims;

namespace Sofia.Web.Controllers;

[Authorize]
[Route("calendar")]
public class CalendarController : Controller
{
    private readonly ICalendarService _calendarService;

    public CalendarController(ICalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    // -----------------------------
    // Календарь
    // -----------------------------
    [HttpGet("")]
    public async Task<IActionResult> Index(int? year, int? month)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var vm = await _calendarService.GetCalendarAsync(userId, year, month);
        return View(vm);
    }

    // -----------------------------
    // Сохранение эмоции
    // -----------------------------
    [HttpPost("save-emotion")]
    public async Task<IActionResult> SaveEmotion([FromBody] SaveEmotionRequest? request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Пользователь не авторизован" });

        if (request == null)
            return Json(new { success = false, message = "Некорректные данные" });

        var result = await _calendarService.SaveEmotionAsync(userId, request);

        return Json(new { success = result.Success, message = result.Message });
    }

    // -----------------------------
    // Детали дня
    // -----------------------------
    [HttpGet("day-details")]
    public async Task<IActionResult> DayDetails(string date)
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Пользователь не авторизован" });

        if (!DateTime.TryParse(date, out var targetDate))
            return Json(new { success = false, message = "Неверный формат даты" });

        var vm = await _calendarService.GetDayDetailsAsync(userId, targetDate);

        if (vm == null)
            return Json(new { success = false, message = "Данные не найдены" });

        return PartialView("_DayDetails", vm);
    }

    // -----------------------------
    // Статистика эмоций
    // -----------------------------
    [HttpGet("emotion-stats")]
    public async Task<IActionResult> EmotionStats(int? days)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var (stats, daysBack) = await _calendarService.GetEmotionStatsAsync(userId, days);

        ViewBag.EmotionStats = stats;
        ViewBag.DaysBack = daysBack;

        return View();
    }
}
