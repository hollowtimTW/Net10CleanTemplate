namespace YourApp.Application.Abstractions;

/// <summary>
/// Represents the current authenticated user/request principal.
/// Implementation lives in the WebApi project (Cookie/JWT/Windows).
/// </summary>
public interface ICurrentUser
{
    /// <summary>Stable user id (employee number, AD sAMAccountName, JWT sub, etc.).</summary>
    string? UserId { get; }

    /// <summary>Display name.</summary>
    string? DisplayName { get; }

    /// <summary>True if any principal is associated with the current request.</summary>
    bool IsAuthenticated { get; }

    /// <summary>Role names (RBAC).</summary>
    IReadOnlyList<string> Roles { get; }

    /// <summary>Caller IP (from X-Forwarded-For or RemoteIpAddress).</summary>
    string? IpAddress { get; }

    /// <summary>Correlation id (TraceIdentifier or inbound header).</summary>
    string? CorrelationId { get; }
}