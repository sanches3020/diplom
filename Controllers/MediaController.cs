using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofia.Web.Services;

namespace Sofia.Web.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly IFileService _fileService;

    public MediaController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [Authorize]
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не получен.");

        var media = await _fileService.SaveAsync(file);

        return Ok(new
        {
            id = media.Id,
            url = "/" + media.FilePath,
            fileName = media.FileName,
            contentType = media.ContentType
        });
    }
}
