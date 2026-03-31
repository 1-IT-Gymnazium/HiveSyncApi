namespace HiveSync.Application.Contracts.Interfaces;

/// <summary>
/// Provides information about the currently authenticated user.
/// Typically used in repositories and services to enforce ownership or track actions.
/// </summary>
public interface ICurrentUserProvider
{
    /// <summary>
    /// Gets the ID of the currently authenticated user.
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// Gets the email of the currently authenticated user, if available.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets the ID of the user's default inbox project.
    /// </summary>
    Guid InboxId { get; }
}
