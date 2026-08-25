namespace YourApp.Identity;

/// <summary>
/// Represents a known user in the system. Concrete implementations come from
/// AD/LDAP/DB — keep this record free of any transport-specific concerns.
/// </summary>
public sealed record UserPrincipal(
    string UserId,
    string DisplayName,
    string? Email,
    IReadOnlyList<string> Roles);