using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Admin;
using Sofia.Web.DTO.Complaints;

[Authorize(Roles = "Admin")]
[Route("admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IComplaintService _complaintService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(
        IAdminService adminService,
        IComplaintService complaintService,
        UserManager<ApplicationUser> userManager)
    {
        _adminService = adminService;
        _complaintService = complaintService;
        _userManager = userManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var users = await _adminService.GetAllUsersAsync();
        var userViewModels = new List<AdminUserViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userViewModels.Add(new AdminUserViewModel { User = user, Roles = roles });
        }

        return View(userViewModels);
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

    // ===================== Жалобы =====================

    /// <summary>
    /// Просмотр всех жалоб
    /// </summary>
    [HttpGet("complaints")]
    public async Task<IActionResult> Complaints(int? status = null)
    {
        var complaints = await _complaintService.GetAllComplaintsAsync(status);
        var stats = await _complaintService.GetComplaintStatsAsync();

        var viewModel = new AdminComplaintsViewModel
        {
            Complaints = complaints,
            Stats = stats,
            StatusFilter = status
        };

        return View(viewModel);
    }

    /// <summary>
    /// Просмотр жалобы
    /// </summary>
    [HttpGet("complaints/{id}")]
    public async Task<IActionResult> ComplaintDetail(int id)
    {
        var complaint = await _complaintService.GetComplaintByIdAsync(id);
        if (complaint == null)
            return NotFound();

        return View(complaint);
    }

    /// <summary>
    /// Обновить статус жалобы (POST)
    /// </summary>
    [HttpPost("complaints/{id}/status")]
    public async Task<IActionResult> UpdateComplaintStatus(int id, [FromForm] UpdateComplaintStatusRequest request)
    {
        var adminId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(adminId))
            return Unauthorized();

        var success = await _complaintService.UpdateComplaintStatusAsync(id, adminId, request);
        if (!success)
            return NotFound();

        await _adminService.LogAsync(adminId, "UpdateComplaintStatus", null, 
            $"Complaint #{id} status changed to {request.Status}");

        return RedirectToAction("ComplaintDetail", new { id });
    }

    /// <summary>
    /// Просмотр жалоб на конкретного пользователя
    /// </summary>
    [HttpGet("complaints/user/{userId}")]
    public async Task<IActionResult> UserComplaints(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var complaints = await _complaintService.GetComplaintsOnUserAsync(userId);

        ViewBag.TargetUser = user;

        return View(complaints);
    }
}
