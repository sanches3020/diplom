using Microsoft.AspNetCore.Mvc;
using Sofia.Web.Models;

namespace Sofia.Web.Controllers;

public class ForumPageController : Controller
{
    private readonly IForumService _forumService;

    public ForumPageController(IForumService forumService)
    {
        _forumService = forumService;
    }

    [HttpGet("/Forum")]
    public async Task<IActionResult> Index()
    {
        var categories = await _forumService.GetCategoriesAsync();
        return View(categories);
    }

    [HttpGet("/Forum/Category/{id}")]
    public async Task<IActionResult> Category(int id)
    {
        var categories = await _forumService.GetCategoriesAsync();
        var category = categories.FirstOrDefault(c => c.Id == id);
        if (category == null)
            return NotFound();

        var threads = await _forumService.GetThreadsByCategoryAsync(id);
        ViewBag.Category = category;
        return View(threads);
    }

    [HttpGet("/Forum/Thread/{id}")]
    public async Task<IActionResult> Thread(int id)
    {
        var thread = await _forumService.GetThreadAsync(id);
        if (thread == null)
            return NotFound();

        return View(thread);
    }

    [HttpGet("/Forum/CreateThread")]
    public async Task<IActionResult> CreateThread()
    {
        ViewBag.Categories = await _forumService.GetCategoriesAsync();
        return View();
    }

    [HttpPost("/Forum/CreateThread")]
    public async Task<IActionResult> CreateThread(int categoryId, string title)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login", "Auth", new { returnUrl = "/Forum/CreateThread" });

        if (string.IsNullOrWhiteSpace(title))
        {
            ModelState.AddModelError(string.Empty, "Введите заголовок темы");
            ViewBag.Categories = await _forumService.GetCategoriesAsync();
            return View();
        }

        await _forumService.CreateThreadAsync(categoryId, title.Trim(), userId);
        return RedirectToAction(nameof(Category), new { id = categoryId });
    }

    [HttpGet("/Forum/CreatePost/{threadId}")]
    public async Task<IActionResult> CreatePost(int threadId)
    {
        var thread = await _forumService.GetThreadAsync(threadId);
        if (thread == null)
            return NotFound();

        ViewBag.Thread = thread;
        return View();
    }

    [HttpPost("/Forum/CreatePost/{threadId}")]
    public async Task<IActionResult> CreatePost(int threadId, string content)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction("Login", "Auth", new { returnUrl = $"/Forum/CreatePost/{threadId}" });

        if (string.IsNullOrWhiteSpace(content))
        {
            ModelState.AddModelError(string.Empty, "Введите текст сообщения");
            var thread = await _forumService.GetThreadAsync(threadId);
            ViewBag.Thread = thread;
            return View();
        }

        await _forumService.CreatePostAsync(threadId, content.Trim(), userId);
        return RedirectToAction(nameof(Thread), new { id = threadId });
    }
}
