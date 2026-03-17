using Microsoft.AspNetCore.Mvc;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Home;
using System.Diagnostics;
using System.Security.Claims;

namespace Sofia.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHomeService _service;

    public HomeController(ILogger<HomeController> logger, IHomeService service)
    {
        _logger = logger;
        _service = service;
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private bool IsPsychologist()
    {
        return User.IsInRole("psychologist");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();

        if (userId != null && IsPsychologist())
        {
            var psychologistId = await _service.GetPsychologistIdForUserAsync(userId);
            if (psychologistId != null)
                return RedirectToAction("Dashboard", "Psychologist", new { id = psychologistId });
        }

        var vm = await _service.GetHomePageDataAsync();
        return View(vm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Companion()
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        return View();
    }

    [HttpPost("api/onboarding/complete")]
    public IActionResult CompleteOnboarding()
    {
        HttpContext.Session.SetString("OnboardingCompleted", "true");
        return Ok();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
