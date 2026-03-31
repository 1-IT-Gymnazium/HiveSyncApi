using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents a refresh token issued to a user for renewing access tokens.
/// </summary>
namespace HiveSync.Application.Contracts.Entities.Identity;

[Table(nameof(RefreshToken))]
public class RefreshToken
{
    /// <summary>
    /// Primary key of the refresh token.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the user to whom this token belongs.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// The actual token string used for refreshing authentication.
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// The instant when the token was created.
    /// </summary>
    public Instant CreatedAt { get; set; }

    /// <summary>
    /// The instant when the token expires and can no longer be used.
    /// </summary>
    public Instant ExpiresAt { get; set; }

    /// <summary>
    /// The instant when the token was revoked, if applicable.
    /// </summary>
    public Instant? RevokedAt { get; set; }

    /// <summary>
    /// Optional information about the request that generated this token, such as IP address or user agent.
    /// </summary>
    public string? RequestInfo { get; set; }
}
