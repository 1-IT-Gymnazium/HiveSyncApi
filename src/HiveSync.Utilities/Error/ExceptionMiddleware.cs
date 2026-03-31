using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HiveSync.Utilities.Error;

/// <summary>
/// Middleware for handling exceptions globally and converting them
/// into structured API error responses.
/// </summary>
/// <remarks>
/// This middleware intercepts exceptions thrown during request processing.
/// - <see cref="HiveSyncException"/> instances are returned with their
///   specific status code and structured errors.
/// - All other unhandled exceptions are logged and returned as a 500
///   Internal Server Error with a generic message.
/// </remarks>
/// <param name="next">
/// The next <see cref="RequestDelegate"/> in the ASP.NET Core pipeline.
/// </param>
/// <param name="logger">
/// The <see cref="ILogger{ExceptionMiddleware}"/> used for logging unexpected errors.
/// </param>
public class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger
    )
{
    /// <summary>
    /// Invokes the middleware, handling any exceptions that occur in the pipeline.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (HiveSyncException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(
                new ApiErrorResponse
                {
                    Errors = ex.Errors
                });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new ApiErrorResponse
            {
                Errors =
                [
                    new ApiErrorItem
                    {
                        Messages =
                        [
                            "Unexpected server error."
                        ]
                    }
                ]
            });
        }
    }
}
