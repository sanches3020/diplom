using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofia.Web.DTO.Calendar;
using Sofia.Web.Services.Interfaces;
using System.Security.Claims;
using System.Text.Json;

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

    [HttpGet("")]
    public async Task<IActionResult> Index(int? year, int? month)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var vm = await _calendarService.GetCalendarAsync(userId, year, month);
        return View(vm);
    }

    [HttpPost("save-emotion")]
    public async Task<IActionResult> SaveEmotion()
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Пользователь не авторизован" });

        var request = await ReadRequestAsync<SaveEmotionRequest>();
        if (request == null)
            return Json(new { success = false, message = "Некорректные данные" });

        var result = await _calendarService.SaveEmotionAsync(userId, request);

        return Json(new { success = result.Success, message = result.Message });
    }

    private async Task<T?> ReadRequestAsync<T>() where T : class
    {
        if (Request.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
        {
            return await JsonSerializer.DeserializeAsync<T>(Request.Body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        if (Request.HasFormContentType)
        {
            var form = await Request.ReadFormAsync();
            var values = form.ToDictionary(k => k.Key, v => (object?)v.Value.ToString());
            var json = JsonSerializer.Serialize(values);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        return null;
    }

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
