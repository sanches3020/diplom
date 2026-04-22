using Microsoft.AspNetCore.Mvc;
using Sofia.Web.DTO.UserTest;
using Sofia.Web.Services.Interfaces;
using System.Security.Claims;

namespace Sofia.Web.Controllers;

[Route("tests")]
public class TestsController : Controller
{
    private readonly IUserTestService _service;

    public TestsController(IUserTestService service)
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
        if (userId == null) return RedirectToAction("Login", "Auth");

        var vm = await _service.GetTestsAsync();
        return View(vm);
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> Analytics()
    {
        var userId = GetUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var vm = await _service.GetAnalyticsAsync();
        return View(vm);
    }

    [HttpGet("analytics/data")]
    public async Task<IActionResult> AnalyticsData(int testId, DateTime from, DateTime to)
    {
        var userId = GetUserId();
        if (userId == null) return Json(new { success = false });

        var data = await _service.GetAnalyticsDataAsync(userId, new UserTestAnalyticsRequest
        {
            TestId = testId,
            From = from,
            To = to
        });

        return Json(data);
    }

    [HttpGet("take/{id}")]
    public async Task<IActionResult> Take(int id)
    {
        var userId = GetUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var vm = await _service.GetTestForTakingAsync(id);
        return vm == null ? NotFound() : View(vm);
    }

    [HttpGet("history/{testId}")]
    public async Task<IActionResult> History(int testId)
    {
        var userId = GetUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var vm = await _service.GetHistoryAsync(userId, testId);
        return vm == null ? NotFound() : View(vm);
    }

    [HttpPost("submit/{id}")]
    public async Task<IActionResult> Submit(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Json(new { success = false });

        var result = await _service.SubmitAsync(userId, id, Request.Form);
        return Json(result);
    }

    [HttpGet("result/{id}")]
    public async Task<IActionResult> Result(int id)
    {
        var userId = GetUserId();
        if (userId == null) return RedirectToAction("Login", "Auth");

        var vm = await _service.GetResultAsync(userId, id);
        return vm == null ? NotFound() : View(vm);
    }
}
