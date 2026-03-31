namespace HiveSync.Api.Controllers.Auth.Models;

public class ResetPasswordModel
{
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
