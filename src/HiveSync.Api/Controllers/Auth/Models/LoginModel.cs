using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HiveSync.Api.Controllers.Auth.Models;

/// <summary>
/// Model used for user login.
/// Contains email and password.
/// </summary>
public class LoginModel
{
    /// <summary>
    /// User's email address.
    /// </summary>
    [Required]
    [property: JsonProperty("email")]
    [EmailAddress]
    public string Email { get; set; } = null!;

    /// <summary>
    /// User's password.
    /// </summary>
    [Required]
    [property: JsonProperty("password")]
    [PasswordPropertyText]
    public string Password { get; set; } = null!;
}
