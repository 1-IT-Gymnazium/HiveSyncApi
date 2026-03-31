namespace HiveSync.Utilities.Settings;

/// <summary>
/// Represents configuration settings required for JWT authentication and token generation.
/// </summary>
/// <remarks>
/// This class is typically bound from application configuration (e.g., appsettings.json)
/// and used to configure token issuance and validation.
/// </remarks>
public class JwtSettings
{
    /// <summary>
    /// Gets or sets the secret key used to sign JWT tokens.
    /// </summary>
    /// <remarks>
    /// This key must be sufficiently long and securely stored.
    /// It is used to create a symmetric security key for HMAC signing.
    /// </remarks>
    public required string SecretKey { get; set; }

    /// <summary>
    /// Gets or sets the expected issuer of the JWT token.
    /// </summary>
    /// <remarks>
    /// This value is validated during token verification.
    /// </remarks>
    public required string Issuer { get; set; }

    /// <summary>
    /// Gets or sets the intended audience of the JWT token.
    /// </summary>
    /// <remarks>
    /// This value is validated during token verification.
    /// </remarks>
    public required string Audience { get; set; }

    /// <summary>
    /// Gets or sets the lifetime of the access token in minutes.
    /// </summary>
    /// <remarks>
    /// Access tokens are short-lived and typically used for API authorization.
    /// </remarks>
    public required int AccessTokenExpirationInMinutes { get; set; }

    /// <summary>
    /// Gets or sets the lifetime of the refresh token in days.
    /// </summary>
    /// <remarks>
    /// Refresh tokens are long-lived and used to issue new access tokens
    /// without requiring the user to re-authenticate.
    /// </remarks>
    public required int RefreshTokenExpirationInDays { get; set; }
}
