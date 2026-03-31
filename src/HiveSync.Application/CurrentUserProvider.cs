using HiveSync.Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HiveSync.Application;

/// <summary>
/// Provides access to information about the currently authenticated user
/// based on claims stored in the HTTP context.
/// 
/// This implementation relies on JWT claims and assumes that:
/// - <see cref="ClaimTypes.NameIdentifier"/> contains the user ID (GUID)
/// - A custom claim "inbox_id" contains the default Inbox project ID (GUID)
/// - <see cref="ClaimTypes.Email"/> contains the user's email
/// 
/// Throws <see cref="UnauthorizedAccessException"/> if required claims
/// are missing or the user is not authenticated.
/// </summary>
public class CurrentUserProvider(
    IHttpContextAccessor httpContextAccessor
) : ICurrentUserProvider
{
    private const string InboxClaimType = "inbox_id";

    /// <summary>
    /// Gets the current authenticated <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when there is no active HTTP context or user.
    /// </exception>
    private ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException();

    /// <summary>
    /// Gets the unique identifier of the currently authenticated user.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the NameIdentifier claim is missing or invalid.
    /// </exception>
    public Guid UserId =>
        Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException());

    /// <summary>
    /// Gets the default Inbox project identifier assigned to the user.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the custom "inbox_id" claim is missing or invalid.
    /// </exception>
    public Guid InboxId =>
        Guid.Parse(
            User.FindFirstValue(InboxClaimType)
            ?? throw new UnauthorizedAccessException("Missing inbox_id claim"));

    /// <summary>
    /// Gets the email address of the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Returns <c>null</c> if the email claim is not present.
    /// </remarks>
    public string? Email =>
        User.FindFirstValue(ClaimTypes.Email);
}
