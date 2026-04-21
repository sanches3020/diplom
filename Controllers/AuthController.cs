using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sofia.Web.Data;
using Sofia.Web.Models;
using Sofia.Web.ViewModels.Auth;

namespace Sofia.Web.Controllers;

[Route("auth")]
public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SofiaDbContext _db;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        SofiaDbContext db)
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

        var loginValue = model.UsernameOrEmail?.Trim() ?? string.Empty;
        ApplicationUser? user = null;

        if (!string.IsNullOrWhiteSpace(loginValue))
        {
            user = loginValue.Contains("@")
                ? await _userManager.FindByEmailAsync(loginValue)
                : await _userManager.FindByNameAsync(loginValue);

            if (user == null && loginValue.Contains("@"))
                user = await _userManager.FindByNameAsync(loginValue);
        }

        if (user == null)
        {
            model.Error = "Пользователь не найден";
            return View(model);
        }

        if (user.IsBlocked)
        {
            model.Error = "Ваш аккаунт заблокирован. Обратитесь к администратору.";
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            model.Password,
            isPersistent: model.RememberMe,
            lockoutOnFailure: false
        );

        if (!result.Succeeded)
        {
            model.Error = "Неверный логин или пароль";
            return View(model);
        }

        await FillSessionAsync(user);
        return RedirectToAction("Index", "Home");
    }

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

        var normalizedUserName = model.Username.Trim();
        var normalizedEmail = model.Email.Trim().ToLowerInvariant();

        if (await _userManager.FindByNameAsync(normalizedUserName) != null)
        {
            model.Error = "Пользователь с таким именем уже существует";
            return View(model);
        }

        if (await _userManager.FindByEmailAsync(normalizedEmail) != null)
        {
            model.Error = "Пользователь с таким email уже существует";
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = normalizedUserName,
            Email = normalizedEmail,
            FullName = model.FullName,
            UserType = model.Role,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            model.Error = string.Join("; ", result.Errors.Select(e => e.Description));
            return View(model);
        }

        // ������ ����, ���� � ���
        if (!await _roleManager.RoleExistsAsync(model.Role))
            await _roleManager.CreateAsync(new ApplicationRole(model.Role));

        // ��������� ����
        await _userManager.AddToRoleAsync(user, model.Role);

        // ���� �������� � ������ �������
        if (model.Role == "psychologist")
        {
            var profile = new Psychologist
            {
                UserId = user.Id,
                Name = model.FullName ?? user.UserName ?? "Психолог",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Psychologists.Add(profile);
            await _db.SaveChangesAsync();
        }

        // вход после регистрации
        await _signInManager.SignInAsync(user, isPersistent: true);
        await FillSessionAsync(user);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("forgot-password")]
    public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var loginValue = model.EmailOrUsername.Trim();
        var user = loginValue.Contains("@")
            ? await _userManager.FindByEmailAsync(loginValue)
            : await _userManager.FindByNameAsync(loginValue);

        if (user == null && loginValue.Contains("@"))
            user = await _userManager.FindByNameAsync(loginValue);

        if (user == null)
        {
            model.Message = "Если пользователь найден, инструкции появятся ниже.";
            return View(model);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        model.Email = user.Email ?? string.Empty;
        model.Message = $"Ссылка для сброса: /auth/reset-password?email={Uri.EscapeDataString(model.Email)}&token={Uri.EscapeDataString(token)}";
        return View(model);
    }

    [HttpGet("reset-password")]
    public IActionResult ResetPassword(string email, string token)
    {
        return View(new ResetPasswordViewModel { Email = email, Token = token });
    }

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

    private async Task FillSessionAsync(ApplicationUser user)
    {
        HttpContext.Session.SetString("UserId", user.Id);
        HttpContext.Session.SetString("Username", user.UserName ?? user.Email ?? "Пользователь");
        HttpContext.Session.SetString("Email", user.Email ?? string.Empty);

        var roles = await _userManager.GetRolesAsync(user);
        var primaryRole = roles.FirstOrDefault() ?? user.UserType ?? "user";
        HttpContext.Session.SetString("UserRole", primaryRole);
    }
}
