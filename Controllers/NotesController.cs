using Microsoft.AspNetCore.Mvc;
using Sofia.Web.DTO.Notes;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Notes;
using System.Security.Claims;

namespace Sofia.Web.Controllers;

[Route("notes")]
public class NotesController : Controller
{
    private readonly INotesService _notesService;

    public NotesController(INotesService notesService)
    {
        _notesService = notesService;
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

        var notes = await _notesService.GetUserNotesAsync(userId);

        var vm = new NotesListViewModel
        {
            Notes = notes
        };

        return View(vm);
    }

    [HttpGet("create")]
    public IActionResult Create(string? date)
    {
        var targetDate = !string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var parsedDate)
            ? parsedDate
            : DateTime.Today;

        var vm = new NoteCreateViewModel
        {
            TargetDate = targetDate
        };

        return View(vm);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateNoteRequest? request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Пользователь не авторизован" });

        if (request == null)
            return Json(new { success = false, message = "Некорректные данные" });

        var result = await _notesService.CreateNoteAsync(userId, request);

        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var note = await _notesService.GetNoteAsync(userId, id);
        if (note == null)
            return NotFound();

        var vm = new NoteEditViewModel
        {
            Note = note
        };

        return View(vm);
    }

    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] UpdateNoteRequest? request)
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Пользователь не авторизован" });

        if (request == null)
            return Json(new { success = false, message = "Некорректные данные" });

        var result = await _notesService.UpdateNoteAsync(userId, id, request);

        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Пользователь не авторизован" });

        var result = await _notesService.DeleteNoteAsync(userId, id);

        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost("toggle-pin/{id}")]
    public async Task<IActionResult> TogglePin(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Пользователь не авторизован" });

        var result = await _notesService.TogglePinAsync(userId, id);

        if (!result.Success)
            return Json(new { success = false, message = result.Message });

        return Json(new
        {
            success = true,
            message = result.Message,
            isPinned = result.IsPinned
        });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var userId = GetUserId();
        if (userId == null)
            return Json(new { success = false, message = "Пользователь не авторизован" });

        var stats = await _notesService.GetStatsAsync(userId);

        if (!stats.Success)
            return Json(new { success = false, message = "Ошибка при получении статистики" });

        return Json(new
        {
            success = true,
            todayNotes = stats.TodayNotes,
            pinnedNotes = stats.PinnedNotes,
            sharedNotes = stats.SharedNotes
        });
    }
}
