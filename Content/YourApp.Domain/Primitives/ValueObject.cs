namespace YourApp.Domain.Primitives;

/// <summary>
/// Base class for value objects. Value objects have no identity; they are equal when their
/// components are equal. Inheriting classes should override <see cref="GetEqualityComponents"/>.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other)
        => other is not null && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    public override bool Equals(object? obj) => obj is ValueObject v && Equals(v);

    public override int GetHashCode()
        => GetEqualityComponents()
            .Aggregate(1, (current, obj) => HashCode.Combine(current, obj?.GetHashCode() ?? 0));

    public static bool operator ==(ValueObject? a, ValueObject? b)
        => a is null ? b is null : a.Equals(b);

    public static bool operator !=(ValueObject? a, ValueObject? b) => !(a == b);
}