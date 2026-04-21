using System.ComponentModel.DataAnnotations;

namespace Sofia.Web.ViewModels.Auth;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Введите имя пользователя")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Имя пользователя должно быть от 3 до 30 символов")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [MinLength(6, ErrorMessage = "Минимальная длина пароля — 6 символов")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтвердите пароль")]
    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите имя")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Выберите роль")]
    public string Role { get; set; } = "user"; // user / psychologist

    public string? Error { get; set; }
}
