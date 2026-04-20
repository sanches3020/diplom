using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sofia.Web.DTO.Complaints;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;

namespace Sofia.Web.Controllers;

[ApiController]
[Route("api/complaints")]
public class ComplaintsController : ControllerBase
{
    private readonly IComplaintService _complaintService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ComplaintsController(
        IComplaintService complaintService,
        UserManager<ApplicationUser> userManager)
    {
        _complaintService = complaintService;
        _userManager = userManager;
    }

    // ===================== User Endpoints =====================

    /// <summary>
    /// Подать жалобу на контент
    /// </summary>
    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> CreateComplaint([FromBody] CreateComplaintRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _complaintService.CreateComplaintAsync(userId, request);
        if (result == null)
        {
            return BadRequest(new { message = "Could not create complaint. Check if you've already complained about this content." });
        }

        return CreatedAtAction(nameof(GetComplaint), new { id = result.Id }, result);
    }

    /// <summary>
    /// Получить жалобу по ID (только для администраторов)
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetComplaint(int id)
    {
        var complaint = await _complaintService.GetComplaintByIdAsync(id);
        if (complaint == null)
            return NotFound();

        return Ok(complaint);
    }

    // ===================== Admin Endpoints =====================

    /// <summary>
    /// Получить все жалобы (только для администраторов)
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpGet("admin/all")]
    public async Task<IActionResult> GetAllComplaints([FromQuery] int? status = null)
    {
        var complaints = await _complaintService.GetAllComplaintsAsync(status);
        return Ok(complaints);
    }

    /// <summary>
    /// Получить жалобы на конкретного пользователя (только для администраторов)
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpGet("admin/user/{targetUserId}")]
    public async Task<IActionResult> GetComplaintsOnUser(string targetUserId)
    {
        var complaints = await _complaintService.GetComplaintsOnUserAsync(targetUserId);
        return Ok(complaints);
    }

    /// <summary>
    /// Обновить статус жалобы (только для администраторов)
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateComplaintStatus(int id, [FromBody] UpdateComplaintStatusRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var adminId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(adminId))
            return Unauthorized();

        var result = await _complaintService.UpdateComplaintStatusAsync(id, adminId, request);
        if (!result)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Получить количество незавершенных жалоб (только для администраторов)
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpGet("admin/unreviewed-count")]
    public async Task<IActionResult> GetUnreviewedCount()
    {
        var count = await _complaintService.GetUnreviewedCountAsync();
        return Ok(new { unreviewed = count });
    }

    /// <summary>
    /// Получить статистику по жалобам (только для администраторов)
    /// </summary>
    [Authorize(Roles = "admin")]
    [HttpGet("admin/stats")]
    public async Task<IActionResult> GetComplaintStats()
    {
        var stats = await _complaintService.GetComplaintStatsAsync();
        return Ok(stats);
    }
}
