using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sofia.Web.Models;
using Sofia.Web.ViewModels.Auth;
using System.Security.Claims;

namespace Sofia.Web.Controllers;

[Route("auth")]
public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _db;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _db = db;
    }

    // -----------------------------
    // LOGIN
    // -----------------------------
    [HttpGet("login")]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            model.Error = "Пользователь не найден";
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            model.Password,
            isPersistent: true,
            lockoutOnFailure: false
        );

        if (!result.Succeeded)
        {
            model.Error = "Неверный пароль";
            return View(model);
        }

        return RedirectToAction("Index", "Home");
    }

    // -----------------------------
    // REGISTER
    // -----------------------------
    [HttpGet("register")]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.Role != "user" && model.Role != "psychologist")
        {
            model.Error = "Некорректная роль";
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            model.Error = string.Join("; ", result.Errors.Select(e => e.Description));
            return View(model);
        }

        // Создаём роль, если её нет
        if (!await _roleManager.RoleExistsAsync(model.Role))
            await _roleManager.CreateAsync(new ApplicationRole(model.Role));

        // Назначаем роль
        await _userManager.AddToRoleAsync(user, model.Role);

        // Если психолог — создаём профиль
        if (model.Role == "psychologist")
        {
            var profile = new Psychologist
            {
                UserId = user.Id,
                Name = model.FullName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Psychologists.Add(profile);
            await _db.SaveChangesAsync();
        }

        // Логиним автоматически
        await _signInManager.SignInAsync(user, isPersistent: true);

        return RedirectToAction("Index", "Home");
    }

    // -----------------------------
    // LOGOUT
    // -----------------------------
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // -----------------------------
    // FORGOT PASSWORD
    // -----------------------------
    [HttpGet("forgot-password")]
    public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            model.Message = "Если пользователь существует — письмо отправлено";
            return View(model);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        model.Message = $"Токен для сброса: {token}";
        return View(model);
    }

    // -----------------------------
    // RESET PASSWORD
    // -----------------------------
    [HttpGet("reset-password")]
    public IActionResult ResetPassword(string email, string token)
        => View(new ResetPasswordViewModel { Email = email, Token = token });

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            model.Error = "Пользователь не найден";
            return View(model);
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

        if (!result.Succeeded)
        {
            model.Error = string.Join("; ", result.Errors.Select(e => e.Description));
            return View(model);
        }

        model.Success = true;
        return View(model);
    }
}
