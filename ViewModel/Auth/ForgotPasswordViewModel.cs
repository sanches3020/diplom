namespace Sofia.Web.ViewModels.Auth;

public class ForgotPasswordViewModel
{
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string? Message { get; set; }
}
