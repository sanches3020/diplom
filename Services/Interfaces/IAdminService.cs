using Sofia.Web.Models;

public interface IAdminService
{
    Task<List<ApplicationUser>> GetAllUsersAsync();
    Task<bool> BlockUserAsync(string userId);
    Task<bool> UnblockUserAsync(string userId);
    Task<bool> DeleteUserAsync(string userId);
    Task LogAsync(string adminId, string action, string? targetUserId = null, string? details = null);
}
