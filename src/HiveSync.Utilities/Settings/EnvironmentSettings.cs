namespace HiveSync.Utilities.Settings;

/// <summary>
/// Represents environment-specific configuration values used by the application,
/// primarily for frontend integration and email communication.
/// </summary>
/// <remarks>
/// This class is typically bound from application configuration (e.g., appsettings.json)
/// and injected using the options pattern.
/// </remarks>
public class EnvironmentSettings
{
    /// <summary>
    /// Gets or sets the base URL of the frontend application.
    /// </summary>
    /// <remarks>
    /// Used when generating absolute links (e.g., email confirmation links).
    /// Example: "https://app.hivesync.io".
    /// </remarks>
    public required string FrontendHostUrl { get; set; }

    /// <summary>
    /// Gets or sets the relative URL path used for email confirmation.
    /// </summary>
    /// <remarks>
    /// This value is appended to <see cref="FrontendHostUrl"/> when constructing
    /// confirmation links sent via email.
    /// Example: "/auth/confirm".
    /// </remarks>
    public required string FrontendConfirmUrl { get; set; }

    /// <summary>
    /// Gets or sets the relative URL path used for password reset.
    /// </summary>
    /// <remarks>
    /// This value is appended to <see cref="FrontendHostUrl"/> when constructing
    /// Reset link sent via email.
    /// Example: "/auth/reset-password".
    /// </remarks>
    public required string FrontendResetPasswordUrl { get; set; }

    /// <summary>
    /// Gets or sets the email address used as the sender for outgoing emails.
    /// </summary>
    /// <remarks>
    /// This address appears in the "From" field of system-generated emails.
    /// </remarks>
    public required string SenderEmail { get; set; }

    /// <summary>
    /// Gets or sets the display name used as the sender name for outgoing emails.
    /// </summary>
    /// <remarks>
    /// This value appears alongside <see cref="SenderEmail"/> in the "From" field.
    /// </remarks>
    public required string SenderName { get; set; }
}
