namespace HiveSync.Application.Contracts.Constants;

/// <summary>
/// Centralized string length constraints used across domain entities.
/// These constants ensure consistent validation rules and database limits.
/// </summary>
public class StringLengthConstants
{
    /// <summary>
    /// Maximum allowed length for email addresses.
    /// Based on RFC standards (local-part + domain).
    /// </summary>
    public const int EmailMaxLength = 320;
    public const int ClientNameLength = 150;

    /// <summary>
    /// Max string length constants for Project entity
    /// </summary>
    public const int ProjectNameLength = 150;

    /// <summary>
    /// Max string length constants for Section entity
    /// </summary>
    public const int SectionNameLength = 150;

    /// <summary>
    /// Max string length constants for Todo entity
    /// </summary>
    public const int TodoNameLength = 150;
    public const int TodoDescriptionLength = 3000;
}
