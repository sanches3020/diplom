using Microsoft.AspNetCore.Mvc;
using Sofia.Web.Services.Interfaces;
using System.Security.Claims;

namespace Sofia.Web.Controllers;

[Route("companion")]
public class CompanionController : Controller
{
    private readonly ICompanionService _service;

    public CompanionController(ICompanionService service)
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

        var vm = await _service.GetCompanionDataAsync(userId);
        return View(vm);
    }

    [HttpPost("feed")]
    public async Task<IActionResult> Feed()
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Пользователь не авторизован" });

        var result = await _service.FeedAsync(userId);
        return Json(new { success = true, happiness = result.Value, message = result.Message, petMood = result.Mood });
    }

    [HttpPost("play")]
    public async Task<IActionResult> Play()
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Пользователь не авторизован" });

        var result = await _service.PlayAsync(userId);
        return Json(new { success = true, energy = result.Value, message = result.Message, petMood = result.Mood });
    }

    [HttpPost("comfort")]
    public async Task<IActionResult> Comfort()
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Пользователь не авторизован" });

        var result = await _service.ComfortAsync(userId);
        return Json(new { success = true, comfort = result.Value, message = result.Message, petMood = result.Mood });
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Пользователь не авторизован" });

        var status = await _service.GetStatusAsync(userId);
        return Json(status);
    }
}
