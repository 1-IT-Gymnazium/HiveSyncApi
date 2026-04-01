namespace HiveSync.Utilities.Error;

/// <summary>
/// Central repository of error messages for HiveSync API.
/// Provides both static messages for common errors and dynamic helpers for field-specific validation.
/// </summary>
/// <remarks>
/// This class is intended to standardize error messages across the application
/// and simplify API error responses. It covers entities such as Project, Section, Todo,
/// general validation, authentication/account issues, refresh token handling, and user confirmation.
/// </remarks>
public static class ErrorMessages
{
    #region Project entity

    /// <summary>Project entity was not found.</summary>
    public static string ProjectNotFound() => "Project not found.";

    /// <summary>Name of the project must be unique.</summary>
    public static string ProjectTitleNotUnique() => "Name of the project must be unique.";

    /// <summary>Name of the project is required.</summary>
    public static string ProjectTitleRequired() => "Name of the project is required.";

    /// <summary>Client for project could not be found.</summary>
    public static string ClientForProjectNotFound() => "This client could not be found.";

    /// <summary>Project could not be deleted.</summary>
    public static string ProjectNotDeleted() => "This project could not be deleted.";

    /// <summary>Inbox project cannot be modified or deleted.</summary>
    public static string InboxProjectProtected() => "Your default project cannot be deleted.";

    #endregion

    #region Section entity

    /// <summary>Section entity was not found.</summary>
    public static string SectionNotFound() => "Section not found.";

    /// <summary>Name of the section must be unique.</summary>
    public static string SectionTitleNotUnique() => "Name of the section must be unique.";

    /// <summary>Name of the section is required.</summary>
    public static string SectionTitleRequired() => "Name of the section is required.";

    /// <summary>Section could not be deleted.</summary>
    public static string SectionNotDeleted() => "This section could not be deleted.";

    /// <summary>Section must have a parental project.</summary>
    public static string SectionParentProjectRequired() => "Section must have a parental project.";

    #endregion

    #region Todo entity

    /// <summary>Task entity was not found.</summary>
    public static string TodoNotFound() => "Task not found.";

    /// <summary>Name of the task is required.</summary>
    public static string TodoSummaryRequired() => "Name of the task is required.";

    /// <summary>Task could not be deleted.</summary>
    public static string TodoNotDeleted() => "This task could not be deleted.";

    /// <summary>Selected priority for this task was not found.</summary>
    public static string TodoPriorityNotFound() => "Selected priority for this task was not found.";

    /// <summary>Selected project for this task was not found.</summary>
    public static string TodoProjectNotFound() => "Selected project for this task was not found.";

    /// <summary>A parental project is required for this task.</summary>
    public static string TodoParentProjectRequired() => "A parental project is required.";

    /// <summary>Selected section for this task was not found.</summary>
    public static string TodoSectionNotFound() => "Selected section for this task was not found.";

    /// <summary>Status for this task is required.</summary>
    public static string TodoStatusIsRequired() => "Status is required.";

    /// <summary>Selected status for this task was not found.</summary>
    public static string TodoStatusNotFound() => "Selected status for this task was not found.";

    #endregion

    #region General validation errors

    /// <summary>Validation failed unexpectedly.</summary>
    public static string ValidationError() => "Validation failed unexpectedly.";

    /// <summary>Email is already in use.</summary>
    public static string UserAlreadyExists() => "This email is already in use.";

    /// <summary>User is not authorized.</summary>
    public static string Unauthorized() => "User is not authorized.";

    /// <summary>Resource not found.</summary>
    public static string NotFound() => "Not Found.";

    /// <summary>Operation is forbidden.</summary>
    public static string Forbidden() => "Forbidden.";

    #endregion

    #region Auth / account

    /// <summary>No account matches this email.</summary>
    public static string EmailDoesNotMatch() => "No account matches this email.";

    /// <summary>Account is locked out.</summary>
    public static string AccountLockedOut() => "Account is locked out.";

    /// <summary>Invalid email or password combination.</summary>
    public static string InvalidEmailOrPassword() => "Invalid email or password.";

    /// <summary>Password does not meet complexity requirements.</summary>
    public static string InvalidPassword() => "Password does not meet complexity requirements.";

    #endregion

    #region Refresh token

    /// <summary>Refresh token not found in request or storage.</summary>
    public static string RefreshTokenNotFound() => "Refresh token not found.";

    /// <summary>Refresh token is invalid or has expired.</summary>
    public static string RefreshTokenInvalidOrExpired() => "Invalid or expired refresh token.";

    /// <summary>User associated with refresh token was not found.</summary>
    public static string UserNotFound() => "User not found.";

    #endregion

    #region User confirmation

    /// <summary>Token provided for confirmation is invalid.</summary>
    public static string InvalidToken() => "Invalid token.";

    /// <summary>Email provided is invalid.</summary>
    public static string InvalidEmail() => "Invalid email.";

    /// <summary>Email has already been confirmed.</summary>
    public static string AlreadyConfirmed() => "Email is already confirmed.";

    #endregion

    #region Helpers for dynamic fields

    /// <summary>Generates a required field error message.</summary>
    /// <param name="field">Name of the field.</param>
    public static string Required(string field) => $"{field} is required.";

    /// <summary>Generates a minimum length error message for a field.</summary>
    /// <param name="field">Name of the field.</param>
    /// <param name="length">Minimum required length.</param>
    public static string MinLength(string field, int length) => $"{field} must be at least {length} characters.";

    /// <summary>Generates a maximum length error message for a field.</summary>
    /// <param name="field">Name of the field.</param>
    /// <param name="length">Maximum allowed length.</param>
    public static string MaxLength(string field, int length) => $"{field} cannot exceed {length} characters.";

    /// <summary>Generates a not found error message for a specific entity.</summary>
    /// <param name="entity">Name of the entity.</param>
    public static string NotFound(string entity) => $"{entity} was not found.";

    /// <summary>Generates an already exists error message for a specific entity.</summary>
    /// <param name="entity">Name of the entity.</param>
    public static string AlreadyExists(string entity) => $"{entity} already exists.";

    /// <summary>Generates an error message indicating that a field value must be unique.</summary>
    /// <param name="field">Name of the field that must be unique.</param>
    public static string NotUnique(string field) => $"{field} must be unique.";

    /// <summary>Generates a not deleted error message for a specific entity.</summary>
    /// <param name="entity">Name of the entity.</param>
    public static string NotDeleted(string entity) => $"{entity} could not be deleted.";

    /// <summary>Returns a standardized message indicating that the provided value is not a valid GUID.</summary>
    /// <param name="field">Name of the field.</param>
    public static string InvalidGuid(string field) => $"{field} must be a valid GUID.";

    public static string DoesNotBelongToProject(string entity) => $"{entity} does not belong to the specified project.";
    #endregion
}
