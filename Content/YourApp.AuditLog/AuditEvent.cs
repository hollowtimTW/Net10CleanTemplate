namespace YourApp.AuditLog;

/// <summary>
/// One audit event captured at a particular moment.
/// </summary>
public sealed record AuditEvent(
    string Action,
    string ResourceType,
    string ResourceId,
    string? UserId,
    string? IpAddress,
    string? CorrelationId,
    DateTime OccurredAtUtc,
    string? PayloadJson);