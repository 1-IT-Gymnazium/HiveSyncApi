using Microsoft.AspNetCore.Http;

namespace HiveSync.Utilities.Error;

/// <summary>
/// Represents an HTTP 401 (Unauthorized) application-specific exception.
/// </summary>
/// <remarks>
/// This exception is intended to be thrown when authentication fails
/// or when a user attempts to access a protected resource without
/// valid credentials. It encapsulates a standardized API error payload
/// containing a single error message.
/// </remarks>
public class UnauthorizedException : HiveSyncException
{

    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class
    /// with the specified error message.
    /// </summary>
    /// <param name="message">
    /// The error message describing the reason for the unauthorized access.
    /// </param>
    /// <remarks>
    /// The HTTP status code is automatically set to 401 (Unauthorized),
    /// and the message is wrapped into a single <see cref="ApiErrorItem"/>.
    /// </remarks>
    public UnauthorizedException(string message)
        : base(
            StatusCodes.Status401Unauthorized,
            new List<ApiErrorItem>
            {
                new() {
                    Messages = [message]
                }
            })
    {
    }
}
