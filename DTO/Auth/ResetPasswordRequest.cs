namespace Sofia.Web.DTO.Auth;

public class ResetPasswordRequest
{
    public string Password { get; set; } = "";
    public string ConfirmPassword { get; set; } = "";
}
