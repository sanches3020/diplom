using Microsoft.AspNetCore.Mvc;
using Sofia.Web.Services;

namespace Sofia.Web.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet("history/{room}")]
    public IActionResult GetHistory(string room = "general")
    {
        var history = _chatService.GetHistory(room);
        return Ok(history);
    }
}
