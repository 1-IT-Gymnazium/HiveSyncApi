namespace HiveSync.Utilities.Error;

/// <summary>
/// Represents a single structured error item for API responses in HiveSync.
/// </summary>
/// <remarks>
/// Each item can either be associated with a specific field (validation errors)
/// or be a global error if <see cref="Field"/> is null. Multiple messages can
/// be included for a single field.
/// </remarks>
public class ApiErrorItem
{
    /// <summary>
    /// Gets or initializes the name of the field that caused the error.
    /// Null indicates a global error not tied to a specific field.
    /// </summary>
    public string? Field { get; init; } // null = global error

    /// <summary>
    /// Gets or initializes the list of error messages associated with this item.
    /// </summary>
    public List<string> Messages { get; init; } = [];
}
