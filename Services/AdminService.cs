using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Models;

public class AdminService : IAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SofiaDbContext _db;

    public AdminService(UserManager<ApplicationUser> userManager, SofiaDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        return await _db.Users.OrderBy(u => u.Email).ToListAsync();
    }

    public async Task<bool> BlockUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.IsBlocked = true;
        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<bool> UnblockUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.IsBlocked = false;
        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        await _userManager.DeleteAsync(user);
        return true;
    }

    public async Task LogAsync(string adminId, string action, string? targetUserId = null, string? details = null)
    {
        var log = new AdminLog
        {
            AdminId = adminId,
            Action = action,
            TargetUserId = targetUserId,
            Details = details
        };

        _db.AdminLogs.Add(log);
        await _db.SaveChangesAsync();
    }
}
