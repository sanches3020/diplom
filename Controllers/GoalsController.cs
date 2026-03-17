using Microsoft.AspNetCore.Mvc;
using Sofia.Web.DTO.Goals;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Goals;
using System.Security.Claims;

namespace Sofia.Web.Controllers;

[Route("goals")]
public class GoalsController : Controller
{
    private readonly IGoalsService _service;

    public GoalsController(IGoalsService service)
    {
        _service = service;
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? sort)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var vm = await _service.GetGoalsAsync(userId, sort);
        return View(vm);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(new GoalCreateViewModel());
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateGoalRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        await _service.CreateGoalAsync(userId, request);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var goal = await _service.GetGoalAsync(userId, id);
        if (goal == null)
            return NotFound();

        return View(new GoalEditViewModel { Goal = goal });
    }

    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(int id, UpdateGoalRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        request.Id = id;
        await _service.UpdateGoalAsync(userId, request);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        await _service.DeleteGoalAsync(userId, id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("update-progress/{id}")]
    public async Task<IActionResult> UpdateProgress(int id, UpdateProgressRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        await _service.UpdateProgressAsync(userId, id, request.Progress);
        return RedirectToAction(nameof(Index));
    }
}
