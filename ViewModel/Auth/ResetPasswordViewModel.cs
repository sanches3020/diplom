namespace Sofia.Web.ViewModels.Auth;

public class ResetPasswordViewModel
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public bool Success { get; set; }
    public string? Error { get; set; }
}
