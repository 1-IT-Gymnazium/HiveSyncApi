using Microsoft.AspNetCore.Http;

namespace HiveSync.Utilities.Error;

/// <summary>
/// Represents an HTTP 403 (Forbidden) application-specific exception.
/// </summary>
/// <remarks>
/// This exception is intended to be thrown when a user is authenticated
/// but does not have permission to access a resource. It encapsulates a
/// standardized API error payload containing a single error message.
/// </remarks>
public class ForbiddenException : HiveSyncException
{
    /// <summary>
     /// Initializes a new instance of the <see cref="ForbiddenException"/> class
     /// with the specified error message.
     /// </summary>
     /// <param name="message">
     /// The error message describing why access is forbidden.
     /// </param>
     /// <remarks>
     /// The HTTP status code is automatically set to 403 (Forbidden),
     /// and the message is wrapped into a single <see cref="ApiErrorItem"/>.
     /// </remarks>
    public ForbiddenException(string message)
        : base(
            StatusCodes.Status403Forbidden,
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
