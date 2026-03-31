using Microsoft.AspNetCore.Http;

namespace HiveSync.Utilities.Error;

/// <summary>
/// Represents an HTTP 400 (Bad Request) exception for validation errors in HiveSync API.
/// </summary>
/// <remarks>
/// This exception is intended to be thrown when a request fails validation.
/// It wraps one or more <see cref="ApiErrorItem"/> instances to provide
/// detailed, structured information about each validation error.
/// </remarks>
/// <param name="errors">
/// A list of <see cref="ApiErrorItem"/> describing the validation errors.
/// </param>
public class ApiValidationException(List<ApiErrorItem> errors)
    : HiveSyncException(StatusCodes.Status400BadRequest, errors)
{
}
