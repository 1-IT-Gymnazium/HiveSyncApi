using System.Security.Claims;

namespace HiveSync.Utilities;

/// <summary>
/// Provides extension methods for extracting strongly-typed information
/// from an authenticated <see cref="ClaimsPrincipal"/> instance.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Retrieves the user's display name from the <see cref="ClaimTypes.Name"/> claim.
    /// </summary>
    /// <param name="user">The <see cref="ClaimsPrincipal"/> representing the current user.</param>
    /// <returns>The value of the <see cref="ClaimTypes.Name"/> claim.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the user is not authenticated.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the required name claim is not present.
    /// </exception>
    public static string GetName(this ClaimsPrincipal user)
    {
        if (user.Identity == null || !user.Identity.IsAuthenticated)
            throw new InvalidOperationException("user not logged in");

        var name = user.Claims.First(x => x.Type == ClaimTypes.Name).Value;
        return name;
    }

    /// <summary>
    /// Retrieves the user's unique identifier from the <see cref="ClaimTypes.NameIdentifier"/> claim.
    /// </summary>
    /// <param name="user">The <see cref="ClaimsPrincipal"/> representing the current user.</param>
    /// <returns>The parsed <see cref="Guid"/> representing the user identifier.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the user is not authenticated.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the required identifier claim is not present.
    /// </exception>
    /// <exception cref="FormatException">
    /// Thrown when the identifier claim value is not a valid <see cref="Guid"/>.
    /// </exception>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        if (user.Identity == null || !user.Identity.IsAuthenticated)
            throw new InvalidOperationException("user not logged in");

        var idString = user.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        return Guid.Parse(idString);
    }
}
