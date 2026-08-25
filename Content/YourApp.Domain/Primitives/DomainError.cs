namespace YourApp.Domain.Primitives;

/// <summary>
/// Structured domain error. Use one of the predefined categories for cross-cutting concerns
/// (auth, validation, conflict, not-found, etc.).
/// </summary>
public sealed record DomainError(string Code, string Message, ErrorCategory Category)
{
    public static DomainError Validation(string message) => new("VALIDATION", message, ErrorCategory.Validation);
    public static DomainError NotFound(string message) => new("NOT_FOUND", message, ErrorCategory.NotFound);
    public static DomainError Conflict(string message) => new("CONFLICT", message, ErrorCategory.Conflict);
    public static DomainError Forbidden(string message) => new("FORBIDDEN", message, ErrorCategory.Forbidden);
    public static DomainError Unauthorized(string message) => new("UNAUTHORIZED", message, ErrorCategory.Unauthorized);
    public static DomainError Infrastructure(string message) => new("INFRASTRUCTURE", message, ErrorCategory.Infrastructure);

    public override string ToString() => $"[{Code}] {Message}";
}

public enum ErrorCategory
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    Infrastructure
}