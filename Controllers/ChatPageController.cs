using Microsoft.AspNetCore.Mvc;

namespace Sofia.Web.Controllers;

public class ChatPageController : Controller
{
    [HttpGet("/Chat")]
    public IActionResult Index()
    {
        return View();
    }
}
