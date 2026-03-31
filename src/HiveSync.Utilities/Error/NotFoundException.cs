using Microsoft.AspNetCore.Http;

namespace HiveSync.Utilities.Error;

/// <summary>
/// Represents an HTTP 404 (Not Found) application-specific exception.
/// </summary>
/// <remarks>
/// This exception is intended to be thrown when a requested resource
/// does not exist. It encapsulates a standardized API error payload
/// containing a single error message.
/// </remarks>
public class NotFoundException : HiveSyncException
{

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class
    /// with the specified error message.
    /// </summary>
    /// <param name="message">
    /// The error message describing the missing resource.
    /// </param>
    /// <remarks>
    /// The HTTP status code is automatically set to 404 (Not Found),
    /// and the message is wrapped into a single <see cref="ApiErrorItem"/>.
    /// </remarks>
    public NotFoundException(string message)
        : base(
            StatusCodes.Status404NotFound,
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
