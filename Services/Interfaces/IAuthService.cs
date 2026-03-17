using Sofia.Web.DTO.Auth;

namespace Sofia.Web.Services.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Message)> SendPasswordResetEmailAsync(string email);
    Task<(bool Success, string Message)> ResetPasswordAsync(string email, string token, string newPassword);
}
