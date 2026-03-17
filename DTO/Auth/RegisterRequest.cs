namespace Sofia.Web.DTO.Auth;

public class RegisterRequest
{
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string ConfirmPassword { get; set; } = "";
    public string Role { get; set; } = "user";

    public string? Specialization { get; set; }
    public string? Description { get; set; }
    public string? Education { get; set; }
    public string? Experience { get; set; }
    public string? Languages { get; set; }
    public string? Methods { get; set; }
    public decimal? PricePerHour { get; set; }
    public string? ContactPhone { get; set; }
}
