using HiveSync.Application.Contracts.Interfaces;
using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiveSync.Application.Contracts.Entities.Identity;

/// <summary>
/// Represents an email message that can be tracked, sent, and stored in the system.
/// Implements tracking, ownership, and base entity interfaces.
/// </summary>
[Table(nameof(EmailMessage))]
public class EmailMessage :
    ITrackable,
    IBaseEntity,
    IOwner
{
    /// <summary>
    /// Primary key of the email message.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The recipient's email address.
    /// </summary>
    public required string RecipientEmail { get; set; }

    /// <summary>
    /// The recipient's display name, if available.
    /// </summary>
    public string? RecipientName { get; set; }

    /// <summary>
    /// The subject line of the email.
    /// </summary>
    public required string Subject { get; set; }

    /// <summary>
    /// The main body content of the email.
    /// </summary>
    public required string Body { get; set; }

    /// <summary>
    /// Indicates whether the email has been sent.
    /// </summary>
    public bool Sent { get; set; }

    /// <summary>
    /// The timestamp when the email record was created.
    /// </summary>
    public Instant CreatedAt { get; set; }

    /// <summary>
    /// The sender's email address.
    /// </summary>
    public required string FromEmail { get; set; }

    /// <summary>
    /// The sender's display name.
    /// </summary>
    public required string FromName { get; set; }

    /// <summary>
    /// The user or system that created this email record.
    /// </summary>
    public string CreatedBy { get; set; } = null!;

    /// <summary>
    /// The timestamp when the email record was last modified.
    /// </summary>
    public Instant ModifiedAt { get; set; }

    /// <summary>
    /// The user or system that last modified this email record.
    /// </summary>
    public string ModifiedBy { get; set; } = null!;

    /// <summary>
    /// The timestamp when the email was soft-deleted, if applicable.
    /// </summary>
    public Instant? DeletedAt { get; set; }

    /// <summary>
    /// The user or system that soft-deleted this email, if applicable.
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    /// The owner of this email message.
    /// </summary>
    public Guid OwnerId { get; set; }
}
