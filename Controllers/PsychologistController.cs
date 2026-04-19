using Microsoft.AspNetCore.Mvc;
using Sofia.Web.DTO.Psychologist;
using Sofia.Web.Services.Interfaces;
using System.Security.Claims;

namespace Sofia.Web.Controllers;

[Route("psychologist")]
public class PsychologistController : Controller
{
    private readonly IPsychologistService _psychologistService;
    private readonly IClientAnalyticsService _clientAnalyticsService;
    private readonly IPsychologistProfileService _psychologistProfileService;
    private readonly IAppointmentsService _appointmentsService;
    private readonly IReviewsService _reviewsService;
    private readonly IClientResultsService _clientResultsService;
    private readonly IScheduleService _scheduleService;

    public PsychologistController(string userId,
        IPsychologistService psychologistService,
        IClientAnalyticsService clientAnalyticsService,
        IPsychologistProfileService psychologistProfileService,
        IAppointmentsService appointmentsService,
        IReviewsService reviewsService,
        IClientResultsService clientResultsService,
        IScheduleService scheduleService)
    {
        _psychologistService = psychologistService;
        _clientAnalyticsService = clientAnalyticsService;
        _psychologistProfileService = psychologistProfileService;
        _appointmentsService = appointmentsService;
        _reviewsService = reviewsService;
        _clientResultsService = clientResultsService;
        _scheduleService = scheduleService;
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private bool IsPsychologist()
    {
        return User.IsInRole("psychologist");
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        if (IsPsychologist())
        {
            var psychologistId = await _psychologistService.GetPsychologistIdForUserAsync(userId);
            if (psychologistId != null)
                return RedirectToAction("Dashboard", new { id = psychologistId.Value });
        }

        var vm = await _psychologistService.GetIndexDataAsync(userId);
        return View(vm);
    }

    [HttpGet("dashboard/{id}")]
    public async Task<IActionResult> Dashboard(int id)
    {
        var userId = GetUserId();
        if (userId == null || !IsPsychologist())
            return RedirectToAction("Login", "Auth");

        var vm = await _clientAnalyticsService.GetDashboardAsync(userId);
        if (vm == null)
            return NotFound();

        return View("PsychologistDashboard", vm);
    }

    [HttpGet("profile/{id}")]
    public async Task<IActionResult> Profile(string userId, int id)
    {
        var vm = await _psychologistProfileService.GetProfileAsync(id);
        return vm == null ? NotFound() : View(vm);
    }

    [HttpPost("book")]
    public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Необходимо войти в систему" });

        var result = await _appointmentsService.BookAppointmentAsync(userId, request);
        return Json(result);
    }

    [HttpGet("reviews")]
    public async Task<IActionResult> Reviews()
    {
        var userId = GetUserId();
        if (userId == null || !IsPsychologist())
            return RedirectToAction("Login", "Auth");

        var vm = await _reviewsService.GetReviewsAsync(userId);
        return vm == null ? NotFound() : View(vm);
    }

    [HttpPost("review/{id}/approve")]
    public async Task<IActionResult> ApproveReview(int id)
    {
        var userId = GetUserId();
        if (userId == null || !IsPsychologist())
            return Json(new { success = false, message = "Доступ запрещён" });

        var result = await _reviewsService.ApproveReviewAsync(userId, id);
        return Json(result);
    }

    [HttpPost("review/{id}/reject")]
    public async Task<IActionResult> RejectReview(int id)
    {
        var userId = GetUserId();
        if (userId == null || !IsPsychologist())
            return Json(new { success = false, message = "Доступ запрещён" });

        var result = await _reviewsService.RejectReviewAsync(userId, id);
        return Json(result);
    }

    [HttpPost("review/{id}/delete")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var userId = GetUserId();
        if (userId == null || !IsPsychologist())
            return Json(new { success = false, message = "Доступ запрещён" });

        var result = await _reviewsService.DeleteReviewAsync(userId, id);
        return Json(result);
    }

    [HttpGet("client/{clientUserId}/results")]
    public async Task<IActionResult> ClientResults(string clientUserId)
    {
        var userId = GetUserId();
        if (userId == null || !IsPsychologist())
            return RedirectToAction("Login", "Auth");

        var vm = await _clientResultsService.GetClientResultsAsync(userId, clientUserId);
        return vm == null ? Forbid() : View(vm);
    }

    [HttpGet("client/{clientUserId}/results/csv")]
    public async Task<IActionResult> ClientResultsCsv(string clientUserId)
    {
        var userId = GetUserId();
        if (userId == null || !IsPsychologist())
            return Json(new { success = false, message = "Неавторизован" });

        var result = await _clientResultsService.GetClientResultsCsvAsync(userId, clientUserId);

        if (!result.Success)
            return Json(new { success = false, message = result.Message });

        return File(result.FileBytes!, "text/csv; charset=utf-8", result.FileName);
    }

    private IActionResult File(object v1, string v2, bool fileName)
    {
        throw new NotImplementedException();
    }

    [HttpGet("schedule")]
    public async Task<IActionResult> Schedule()
    {
        var userId = GetUserId();
        if (userId == null || !IsPsychologist())
            return RedirectToAction("Login", "Auth");

        var vm = await _scheduleService.GetScheduleAsync(userId);
        return vm == null ? NotFound() : View(vm);
    }

    [HttpPost("schedule/add")]
    public async Task<IActionResult> AddSchedule(AddScheduleRequest request)
    {
        var userId = GetUserId();
        if (userId == null || !IsPsychologist())
            return Json(new { success = false, message = "Доступ запрещён" });

        var result = await _scheduleService.AddScheduleAsync(userId, request);
        return Json(result);
    }

    [HttpPost("schedule/remove")]
    public async Task<IActionResult> RemoveSchedule(int scheduleId)
    {
        var userId = GetUserId();
        if (userId == null || !IsPsychologist())
            return Json(new { success = false, message = "Доступ запрещён" });

        var result = await _scheduleService.RemoveScheduleAsync(userId, scheduleId);
        return Json(result);
    }

    [HttpPost("schedule/add-time-slot")]
    public async Task<IActionResult> AddTimeSlot([FromBody] AddTimeSlotRequest request)
    {
        var userId = GetUserId();
        if (userId == null || !IsPsychologist())
            return Json(new { success = false, message = "Доступ запрещён" });

        var result = await _scheduleService.AddTimeSlotsAsync(userId, request);
        return Json(result);
    }
}
