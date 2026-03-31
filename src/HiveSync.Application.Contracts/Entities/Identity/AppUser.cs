using HiveSync.Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Identity;
using NodaTime;

namespace HiveSync.Application.Contracts.Entities.Identity;

/// <summary>
/// Represents an application user in the system.
/// Inherits from IdentityUser with Guid as the primary key.
/// Implements ITrackable for auditing purposes.
/// </summary>
public class AppUser : IdentityUser<Guid>, ITrackable
{
    /// <summary>
    /// Display name of the user.
    /// </summary>
    public string DisplayName { get; set; } = null!;

    /// <summary>
    /// The ID of the user's inbox (used for default project, todos, etc.).
    /// </summary>
    public Guid InboxId { get; set; }

    /// <summary>
    /// The timestamp when the user was created.
    /// </summary>
    public Instant CreatedAt { get; set; }

    /// <summary>
    /// The user or system that created this user record.
    /// </summary>
    public string CreatedBy { get; set; } = null!;

    /// <summary>
    /// The timestamp when the user record was last modified.
    /// </summary>
    public Instant ModifiedAt { get; set; }

    /// <summary>
    /// The user or system that last modified this user record.
    /// </summary>
    public string ModifiedBy { get; set; } = null!;

    /// <summary>
    /// The timestamp when the user was soft-deleted, if applicable.
    /// </summary>
    public Instant? DeletedAt { get; set; }

    /// <summary>
    /// The user or system that soft-deleted this user, if applicable.
    /// </summary>
    public string? DeletedBy { get; set; }
}
