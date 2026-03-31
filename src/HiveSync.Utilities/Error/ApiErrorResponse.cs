namespace HiveSync.Utilities.Error;

/// <summary>
/// Standardized response model for returning API errors in HiveSync.
/// </summary>
/// <remarks>
/// Encapsulates a collection of <see cref="ApiErrorItem"/> instances
/// to provide structured error information in HTTP responses.
/// Typically used by the <see cref="ExceptionMiddleware"/> to return
/// errors to clients in a consistent format.
/// </remarks>
public class ApiErrorResponse
{
    /// <summary>
    /// Gets or initializes the list of structured error items.
    /// </summary>
    public List<ApiErrorItem> Errors { get; init; } = [];
}
