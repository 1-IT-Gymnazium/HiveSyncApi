using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HiveSync.Api.Controllers.Auth.Models;

/// <summary>
/// Model used for email confirmation requests.
/// Contains the user's email and the confirmation token.
/// </summary>
public class TokenModel
{
    /// <summary>
    /// Email address of the user whose email is being confirmed.
    /// </summary>
    [EmailAddress]
    [property: JsonProperty("email")]
    [Required]
    public string Email { get; set; } = null!;

    /// <summary>
    /// Email confirmation token generated during registration.
    /// </summary>
    [Required]
    [property: JsonProperty("token")]
    public string Token { get; set; } = null!;
}
