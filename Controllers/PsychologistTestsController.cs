using Microsoft.AspNetCore.Mvc;
using Sofia.Web.DTO.Tests;
using Sofia.Web.Services.Interfaces;
using System.Security.Claims;

namespace Sofia.Web.Controllers;

[Route("psychologist/tests")]
public class PsychologistTestsController : Controller
{
    private readonly ITestsService _testsService;

    public PsychologistTestsController(ITestsService testsService)
    {
        _testsService = testsService;
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private bool IsPsychologist()
    {
        return User.IsInRole("psychologist");
    }

    // ---------------------------------------------------------
    // LIST TESTS
    // ---------------------------------------------------------
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (userId == null || !IsPsychologist())
            return RedirectToAction("Login", "Auth");

        var vm = await _testsService.GetTestsAsync(userId);
        if (vm == null)
            return Forbid();

        return View(vm);
    }

    // ---------------------------------------------------------
    // CREATE TEST (VIEW)
    // ---------------------------------------------------------
    [HttpGet("create")]
    public IActionResult Create()
    {
        if (!IsPsychologist())
            return RedirectToAction("Login", "Auth");

        return View();
    }

    // ---------------------------------------------------------
    // CREATE TEST (POST)
    // ---------------------------------------------------------
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTestRequest request)
    {
        var userId = GetUserId();
        if (userId == null || !IsPsychologist())
            return Json(new { success = false, message = "Доступ запрещён" });

        var result = await _testsService.CreateTestAsync(userId, request);
        return Json(result);
    }

    // ---------------------------------------------------------
    // DELETE TEST
    // ---------------------------------------------------------
    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId == null || !IsPsychologist())
            return Json(new { success = false, message = "Доступ запрещён" });

        var result = await _testsService.DeleteTestAsync(userId, id);
        return Json(result);
    }
}
