using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.DTO.Auth;
using Sofia.Web.Helpers;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;

namespace Sofia.Web.Services;

public class AuthService : IAuthService
{
    private readonly SofiaDbContext _context;

    public AuthService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message, int? UserId, string? Role)> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

        if (user == null || !PasswordHasher.Verify(request.Password, user.Password))
            return (false, "Неверное имя пользователя или пароль", null, null);

        return (true, "Успешный вход", user.Id, user.Role);
    }

    public async Task<(bool Success, string Message, int? UserId, string? Role, int? PsychologistId)> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            return (false, "Пользователь с таким именем уже существует", null, null, null);

        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return (false, "Пользователь с таким email уже существует", null, null, null);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = PasswordHasher.Hash(request.Password),
            Role = request.Role,
            CreatedAt = DateTime.Now
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

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

            // базовое расписание
            var schedules = Enumerable.Range(1, 5).Select(day => new PsychologistSchedule
            {
                PsychologistId = psychologist.Id,
                DayOfWeek = (DayOfWeek)day,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(18, 0, 0),
                IsAvailable = true,
                CreatedAt = DateTime.Now
            });

            _context.PsychologistSchedules.AddRange(schedules);
            await _context.SaveChangesAsync();
        }

        return (true, "Регистрация успешна", user.Id, user.Role, psychologistId);
    }

    public async Task<(bool Success, string Message, int? UserId, string? Email)> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                (u.Email == request.EmailOrUsername || u.Username == request.EmailOrUsername)
                && u.IsActive);

        if (user == null)
            return (true, "Если аккаунт существует, мы отправим инструкции", null, null);

        return (true, "Инструкции отправлены", user.Id, user.Email);
    }

    public async Task<(bool Success, string Message)> ResetPasswordAsync(int userId, ResetPasswordRequest request)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return (false, "Пользователь не найден");

        user.Password = PasswordHasher.Hash(request.Password);
        await _context.SaveChangesAsync();

        return (true, "Пароль успешно изменён");
    }
}
