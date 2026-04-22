using Microsoft.AspNetCore.Mvc;
using Sofia.Web.Services.Interfaces;
using System.Security.Claims;

namespace Sofia.Web.Controllers;

[Route("stats")]
public class StatsController : Controller
{
    private readonly IStatsService _statsService;

    public StatsController(IStatsService statsService)
    {
        _statsService = statsService;
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(int? days)
    {
        var userId = GetUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var vm = await _statsService.GetStatsAsync(userId, days ?? 30);
        return View(vm);
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(int? days)
    {
        var userId = GetUserId();
        if (userId == null) return Json(new { success = false });

        var bytes = await _statsService.ExportCsvAsync(userId, days ?? 30);
        return File(bytes, "text/csv", $"sofia_export_{DateTime.UtcNow:yyyy-MM-dd}.csv");
    }

    [HttpGet("insights")]
    public async Task<IActionResult> Insights()
    {
        var userId = GetUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var vm = await _statsService.GetInsightsAsync(userId);

        if (Request.Query["format"] == "json")
            return Json(vm);

        return View(vm);
    }

    [HttpGet("report")]
    public async Task<IActionResult> Report(int? days, string? format)
    {
        var userId = GetUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var vm = await _statsService.GenerateReportAsync(userId, days ?? 30, format ?? "html");
        return View("Report", vm);
    }
}
