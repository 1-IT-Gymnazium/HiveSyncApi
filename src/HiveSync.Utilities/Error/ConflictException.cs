using Microsoft.AspNetCore.Http;

namespace HiveSync.Utilities.Error;

/// <summary>
/// Represents an HTTP 409 (Conflict) application-specific exception.
/// </summary>
/// <remarks>
/// This exception is intended to be thrown when a request cannot be completed
/// due to a conflict with the current state of the target resource.
/// It encapsulates a standardized API error payload containing a single error message.
/// </remarks>
public class ConflictException : HiveSyncException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class
    /// with the specified error message.
    /// </summary>
    /// <param name="message">
    /// The error message describing the nature of the conflict.
    /// </param>
    /// <remarks>
    /// The HTTP status code is automatically set to 409 (Conflict),
    /// and the message is wrapped into a single <see cref="ApiErrorItem"/>.
    /// </remarks>
    public ConflictException(string message)
        : base(
            StatusCodes.Status409Conflict,
            new List<ApiErrorItem>
            {
                new()
                {
                    Messages = [message]
                }
            })
    {
    }
}
