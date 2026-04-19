using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.DTO.Auth;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;

namespace Sofia.Web.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly SofiaDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        SofiaDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }
    // LOGIN
    // -----------------------------
    public async Task<(bool Success, string Message, string? UserId, string? Role)> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.UserName == request.Username);

        if (user == null)
            return (false, "Неверное имя пользователя или пароль", null, null);

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
            return (false, "Неверное имя пользователя или пароль", null, null);

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();

        return (true, "Успешный вход", user.Id, role);
    }

    // -----------------------------
    // REGISTER
    // -----------------------------
    public async Task<(bool Success, string Message, string? UserId, string? Role, int? PsychologistId)> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser != null)
            return (false, "Пользователь с таким именем уже существует", null, null, null);

        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
            return (false, string.Join("; ", createResult.Errors.Select(e => e.Description)), null, null, null);

        await _userManager.AddToRoleAsync(user, request.Role);

        int? psychologistId = null;

        if (request.Role == "psychologist")
        {
            var psychologist = new Psychologist
            {
                Name = request.Username,
                UserId = user.Id,
                Specialization = request.Specialization ?? "",
                Description = request.Description ?? "",
                Education = request.Education ?? "",
                Experience = request.Experience ?? "",
                Languages = request.Languages,
                Methods = request.Methods,
                PricePerHour = request.PricePerHour ?? 3000,
                ContactPhone = request.ContactPhone,
                ContactEmail = request.Email,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Psychologists.Add(psychologist);
            await _context.SaveChangesAsync();

            psychologistId = psychologist.Id;
        }

        return (true, "Регистрация успешна", user.Id, request.Role, psychologistId);
    }

    // -----------------------------
    // SEND PASSWORD RESET EMAIL
    // -----------------------------
    public async Task<(bool Success, string Message)> SendPasswordResetEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return (false, "Пользователь с таким email не найден");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);

        // Формируем ссылку (замени домен на свой)
        var resetLink = $"https://your-domain.com/auth/reset-password?email={email}&token={encodedToken}";

        // TODO: отправка email через EmailService
        Console.WriteLine($"Password reset link for {email}: {resetLink}");

        return (true, "Ссылка для сброса пароля отправлена");
    }

    // -----------------------------
    // RESET PASSWORD
    // -----------------------------

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
