using Microsoft.AspNetCore.Mvc;
using Sofia.Web.Services.Interfaces;
using System.Security.Claims;

namespace Sofia.Web.Controllers;

[Route("practices")]
public class PracticesController : Controller
{
    private readonly IPracticesService _service;

    public PracticesController(IPracticesService service)
    {
        _service = service;
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? category, int? duration)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var vm = await _service.GetPracticesAsync(category, duration);
        return View(vm);
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var practice = await _service.GetPracticeAsync(id);
        if (practice == null)
            return NotFound();

        return View(practice);
    }

    [HttpPost("start/{id}")]
    public async Task<IActionResult> Start(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var practice = await _service.GetPracticeAsync(id);
        if (practice == null)
            return NotFound();

        TempData["SuccessMessage"] = $"Практика '{practice.Name}' начата!";
        return RedirectToAction(nameof(Details), new { id });
    }
}
