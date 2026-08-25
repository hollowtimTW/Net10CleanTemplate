namespace YourApp.Application.Abstractions;

/// <summary>
/// Abstraction over the system clock. Inject this instead of using DateTime.UtcNow directly,
/// so time-sensitive code is testable.
/// </summary>
public interface IDateTime
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}

public sealed class SystemDateTime : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}