namespace HiveSync.Utilities.Error;

/// <summary>
/// Provides a fluent builder for constructing structured validation error collections.
/// </summary>
/// <remarks>
/// This builder aggregates validation messages per field and produces
/// a list of <see cref="ApiErrorItem"/> instances suitable for API responses.
/// </remarks>
public class ValidationErrorBuilder
{
    private readonly List<ApiErrorItem> _errors = [];

    /// <summary>
    /// Adds a validation error message for the specified field.
    /// </summary>
    /// <param name="field">
    /// The name of the field associated with the validation error.
    /// </param>
    /// <param name="message">
    /// The validation error message to associate with the field.
    /// </param>
    /// <returns>
    /// The current <see cref="ValidationErrorBuilder"/> instance to allow method chaining.
    /// </returns>
    /// <remarks>
    /// If an error entry for the specified field already exists,
    /// the message is appended to its message collection.
    /// Otherwise, a new error entry is created.
    /// </remarks>
    public ValidationErrorBuilder Add(string field, string message)
    {
        var item = _errors.FirstOrDefault(e => e.Field == field);

        if (item != null)
            item.Messages.Add(message);

        else
        {
            _errors.Add(new ApiErrorItem
            {
                Field = field,
                Messages = [message]
            });
        }

        return this;
    }

    /// <summary>
    /// Gets a value indicating whether any validation errors have been added.
    /// </summary>
    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    /// Builds and returns the aggregated validation errors.
    /// </summary>
    /// <returns>
    /// A list of <see cref="ApiErrorItem"/> instances representing
    /// all collected validation errors.
    /// </returns>
    /// <remarks>
    /// The returned list is the internal collection; further modifications
    /// to it will affect the builder state.
    /// </remarks>
    public List<ApiErrorItem> Build() => _errors;
}
