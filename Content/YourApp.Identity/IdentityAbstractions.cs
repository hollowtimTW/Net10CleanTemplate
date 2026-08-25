namespace YourApp.Identity;

/// <summary>
/// Resolves a user from an external identity store (AD / LDAP / DB).
/// Concrete implementations registered in WebApi.
/// </summary>
public interface IUserDirectory
{
    ValueTask<UserPrincipal?> FindByIdAsync(string userId, CancellationToken ct = default);
    ValueTask<UserPrincipal?> FindByNameAsync(string userName, CancellationToken ct = default);
}

/// <summary>
/// Issues and validates JWTs for API authentication.
/// </summary>
public interface IJwtIssuer
{
    string Issue(UserPrincipal principal, TimeSpan? lifetime = null);
    UserPrincipal? Validate(string token);
}

/// <summary>
/// Emergency override — allows privileged users to break the normal permission model
/// in critical situations (e.g. trauma resuscitation). Always audited.
/// </summary>
public interface IBreakGlassService
{
    ValueTask<string> ActivateAsync(string userId, string reason, TimeSpan duration, CancellationToken ct = default);
    ValueTask RevokeAsync(string token, CancellationToken ct = default);
}