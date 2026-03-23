namespace Layla.Core.Common;

/// <summary>
/// Standard error codes for API responses.
/// Used in Result&lt;T&gt; to enable type-safe error handling and consistent HTTP status mapping.
/// </summary>
public enum ErrorCode
{
    // Validation errors (400)
    ValidationFailed = 400,
    InvalidInput = 401,
    DuplicateEmail = 409,
    InvalidRole = 402,

    // Authentication errors (401)
    Unauthorized = 1001,
    InvalidCredentials = 1002,
    AccountLocked = 1003,
    SessionExpired = 1004,
    InvalidToken = 1005,

    // Authorization errors (403)
    Forbidden = 2001,
    InsufficientPermissions = 2002,

    // Not found errors (404)
    NotFound = 3001,
    ProjectNotFound = 3002,
    UserNotFound = 3003,
    CollaboratorNotFound = 3004,

    // Conflict errors (409)
    AlreadyExists = 4001,
    AlreadyMember = 4002,

    // Server errors (500)
    InternalError = 5001,
    DatabaseError = 5002,
    MessagingError = 5003,
}

/// <summary>
/// Maps ErrorCode enum values to HTTP status codes and user-friendly messages.
/// </summary>
public static class ErrorCodeExtensions
{
    /// <summary>Gets the HTTP status code for an error code.</summary>
    public static int GetStatusCode(this ErrorCode code) => (int)code switch
    {
        >= 400 and < 500 => (int)code,
        _ => code switch
        {
            ErrorCode.Unauthorized or ErrorCode.InvalidCredentials or
            ErrorCode.AccountLocked or ErrorCode.SessionExpired or
            ErrorCode.InvalidToken => 401,

            ErrorCode.Forbidden or ErrorCode.InsufficientPermissions => 403,

            ErrorCode.ProjectNotFound or ErrorCode.UserNotFound or
            ErrorCode.CollaboratorNotFound => 404,

            ErrorCode.AlreadyExists or ErrorCode.AlreadyMember => 409,

            ErrorCode.InternalError or ErrorCode.DatabaseError or
            ErrorCode.MessagingError => 500,

            _ => 400,
        }
    };

    /// <summary>Gets a user-friendly error message.</summary>
    public static string GetMessage(this ErrorCode code) => code switch
    {
        ErrorCode.ValidationFailed => "Validation failed. Please check your input.",
        ErrorCode.InvalidInput => "Invalid input provided.",
        ErrorCode.DuplicateEmail => "Email is already registered.",
        ErrorCode.InvalidRole => "Invalid role specified.",

        ErrorCode.Unauthorized => "Unauthorized. Please login.",
        ErrorCode.InvalidCredentials => "Invalid email or password.",
        ErrorCode.AccountLocked => "Account is locked due to multiple failed attempts.",
        ErrorCode.SessionExpired => "Session expired. User logged in from another device.",
        ErrorCode.InvalidToken => "Invalid or malformed token.",

        ErrorCode.Forbidden => "Access denied.",
        ErrorCode.InsufficientPermissions => "You do not have permission to perform this action.",

        ErrorCode.ProjectNotFound => "Project not found.",
        ErrorCode.UserNotFound => "User not found.",
        ErrorCode.CollaboratorNotFound => "Collaborator not found.",

        ErrorCode.AlreadyExists => "Resource already exists.",
        ErrorCode.AlreadyMember => "You are already a member of this project.",

        ErrorCode.InternalError => "An internal error occurred. Please try again later.",
        ErrorCode.DatabaseError => "A database error occurred. Please try again later.",
        ErrorCode.MessagingError => "A messaging error occurred. Please try again later.",

        _ => "An unknown error occurred.",
    };
}
