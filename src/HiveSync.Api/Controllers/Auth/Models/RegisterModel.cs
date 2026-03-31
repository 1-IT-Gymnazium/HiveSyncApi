using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HiveSync.Api.Controllers.Auth.Models;

/// <summary>
/// Model used for user registration.
/// Contains email, password, and display name.
/// </summary>
public class RegisterModel
{
    /// <summary>
    /// User's email address.
    /// </summary>
    [Required]
    [property: JsonProperty("email")]
    [EmailAddress]
    public string Email { get; set; } = null!;

    /// <summary>
    /// User's password. Should follow configured password policy.
    /// </summary>
    [Required]
    [property: JsonProperty("password")]
    [PasswordPropertyText]
    public string Password { get; set; } = null!;

    /// <summary>
    /// Display name for the user.
    /// </summary>
    [Required]
    [property: JsonProperty("displayName")]
    public string DisplayName { get; set; } = null!;
}
