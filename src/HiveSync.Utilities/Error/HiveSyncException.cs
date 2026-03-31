namespace HiveSync.Utilities.Error;

/// <summary>
/// Base class for all application-specific exceptions in HiveSync,
/// providing structured error information for API responses.
/// </summary>
/// <remarks>
/// Inherits from <see cref="Exception"/> and includes an HTTP status code
/// and a collection of <see cref="ApiErrorItem"/> instances representing
/// detailed error messages. Derived exceptions typically correspond to
/// specific HTTP status codes like 401, 404, or 409.
/// </remarks>
/// <param name="statusCode">
/// The HTTP status code associated with the exception.
/// </param>
/// <param name="errors">
/// A collection of <see cref="ApiErrorItem"/> representing one or more
/// structured error messages.
/// </param>
public abstract class HiveSyncException(
    int statusCode,
    List<ApiErrorItem> errors
    ) : Exception
{
    /// <summary>
    /// Gets the HTTP status code associated with this exception.
    /// </summary>
    public int StatusCode { get; } = statusCode;

    /// <summary>
    /// Gets the collection of structured error messages.
    /// </summary>
    public List<ApiErrorItem> Errors { get; } = errors;
}
