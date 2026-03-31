using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static HiveSync.Application.Contracts.Constants.StringLengthConstants;
using NodaTime;
using HiveSync.Application.Contracts.Interfaces;

namespace HiveSync.Application.Contracts.Entities.Business;

/// <summary>
/// Represents a client entity that can own multiple projects.
/// Supports auditing, ownership through <see cref="IBaseEntity"/>.
/// </summary>
[Table(nameof(Client))]
public class Client : IBaseEntity
{
    /// <summary>
    /// Unique identifier of the client.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Display name of the client.
    /// </summary>
    [MaxLength(ClientNameLength)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Identifier of the user who owns this client entity.
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Collection of projects associated with this client.
    /// </summary>
    public ICollection<Project> Projects { get; set; } = new HashSet<Project>();

    /// <summary>
    /// Timestamp when the client was created.
    /// </summary>
    public Instant CreatedAt { get; set; }

    /// <summary>
    /// Username or system identifier that created the client.
    /// </summary>
    public string CreatedBy { get; set; } = null!;

    /// <summary>
    /// Timestamp of the last modification.
    /// </summary>
    public Instant ModifiedAt { get; set; }

    /// <summary>
    /// Username or system identifier that last modified the client.
    /// </summary>
    public string ModifiedBy { get; set; } = null!;

    /// <summary>
    /// Timestamp when the client was soft-deleted, if applicable.
    /// </summary>
    public Instant? DeletedAt { get; set; }

    /// <summary>
    /// Username or system identifier that performed the deletion, if applicable.
    /// </summary>
    public string? DeletedBy { get; set; }
}
