using Microsoft.AspNetCore.Identity;
using Sofia.Web.DTO.Auth;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;

namespace Sofia.Web.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Success, string Message)> SendPasswordResetEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return (true, "Если аккаунт существует, мы отправим инструкции на почту");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // TODO: Подключить сервис отправки почты и передавать ссылку с токеном
        return (true, $"Токен для сброса пароля: {token}");
    }

    public async Task<(bool Success, string Message)> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return (false, "Пользователь не найден");

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        return (true, "Пароль успешно изменён");
    }
}
