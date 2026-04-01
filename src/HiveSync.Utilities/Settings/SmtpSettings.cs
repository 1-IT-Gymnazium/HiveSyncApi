namespace HiveSync.Utilities.Settings;

/// <summary>
/// Represents SMTP configuration settings used for sending emails.
/// </summary>
/// <remarks>
/// This class is typically bound from application configuration (e.g., appsettings.json)
/// and injected via the options pattern.
/// </remarks>
public class SmtpSettings
{
    /// <summary>
    /// Gets or sets the SMTP server host address.
    /// </summary>
    /// <remarks>
    /// Example: "smtp.gmail.com".
    /// </remarks>
    public required string Host { get; set; }

    /// <summary>
    /// Gets or sets the SMTP server port.
    /// </summary>
    /// <remarks>
    /// Common values are 25, 465 (SSL), or 587 (TLS).
    /// </remarks>
    ///
    public required int Port { get; set; }

    /// <summary>
    /// Gets or sets the username used for SMTP authentication.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the password used for SMTP authentication.
    /// </summary>
    /// <remarks>
    /// This value should be stored securely (e.g., user secrets, environment variables, or vault).
    /// </remarks>
    public required string Password { get; set; }
    public required string ApiKey { get; set; }
}
