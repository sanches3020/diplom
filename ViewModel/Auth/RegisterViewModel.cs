using System.ComponentModel.DataAnnotations;

namespace Sofia.Web.ViewModels.Auth;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Введите пароль")]
    [MinLength(6, ErrorMessage = "Минимальная длина пароля — 6 символов")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Введите имя")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Выберите роль")]
    public string Role { get; set; } // user / psychologist

    public string? Error { get; set; }
}
