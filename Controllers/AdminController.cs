using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;

[Authorize(Roles = "admin")]
[Route("admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(IAdminService adminService, UserManager<ApplicationUser> userManager)
    {
        _adminService = adminService;
        _userManager = userManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var users = await _adminService.GetAllUsersAsync();
        return View(users);
    }

    [HttpPost("block/{id}")]
    public async Task<IActionResult> Block(string id)
    {
        var adminId = _userManager.GetUserId(User);

        if (await _adminService.BlockUserAsync(id))
            await _adminService.LogAsync(adminId!, "BlockUser", id);

        return RedirectToAction("Index");
    }

    [HttpPost("unblock/{id}")]
    public async Task<IActionResult> Unblock(string id)
    {
        var adminId = _userManager.GetUserId(User);

        if (await _adminService.UnblockUserAsync(id))
            await _adminService.LogAsync(adminId!, "UnblockUser", id);

        return RedirectToAction("Index");
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var adminId = _userManager.GetUserId(User);

        if (await _adminService.DeleteUserAsync(id))
            await _adminService.LogAsync(adminId!, "DeleteUser", id);

        return RedirectToAction("Index");
    }
}
