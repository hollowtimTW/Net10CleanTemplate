namespace YourApp.Domain.Primitives;

/// <summary>
/// Base for strongly-typed identity (e.g. CustomerId, OrderId). Prevents passing the wrong ID type.
/// </summary>
public abstract record StronglyTypedId<T>(T Value) where T : notnull
{
    public override string ToString() => Value.ToString() ?? string.Empty;
}

public abstract record GuidId(Guid Value) : StronglyTypedId<Guid>(Value)
{
    public static implicit operator Guid(GuidId id) => id.Value;
}

public abstract record LongId(long Value) : StronglyTypedId<long>(Value)
{
    public static implicit operator long(LongId id) => id.Value;
}