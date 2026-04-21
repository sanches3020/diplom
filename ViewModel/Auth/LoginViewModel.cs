namespace Sofia.Web.ViewModels.Auth;

public class LoginViewModel
{
    public string UsernameOrEmail { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = true;

    public string? Error { get; set; }
}
