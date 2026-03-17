using Microsoft.AspNetCore.Mvc;
using Sofia.Web.Services.Interfaces;
using System.Security.Claims;

namespace Sofia.Web.Controllers;

[Route("choose-psychologist")]
public class ChoosePsychologistController : Controller
{
    private readonly IChoosePsychologistService _service;

    public ChoosePsychologistController(IChoosePsychologistService service)
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
        var vm = await _service.GetActivePsychologistsAsync();
        return View(vm);
    }
}
