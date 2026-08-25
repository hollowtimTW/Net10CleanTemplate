using System.Diagnostics.CodeAnalysis;

namespace YourApp.Domain.Primitives;

/// <summary>
/// Defensive precondition checks. Throw early with precise exception types — no silent failures.
/// </summary>
public static class Guard
{
    public static T NotNull<T>(T? value, string paramName) where T : class
    {
        ArgumentNullException.ThrowIfNull(value, paramName);
        return value;
    }

    public static string NotNullOrWhiteSpace(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null, empty, or whitespace.", paramName);
        return value;
    }

    public static T NotDefault<T>(T value, string paramName) where T : struct
    {
        if (value.Equals(default(T)))
            throw new ArgumentException("Value cannot be the default value.", paramName);
        return value;
    }

    public static T Positive<T>(T value, string paramName) where T : IComparable<T>
    {
        if (value.CompareTo(default) <= 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be positive.");
        return value;
    }

    public static IReadOnlyList<T> NotEmpty<T>(IReadOnlyList<T>? value, string paramName)
    {
        ArgumentNullException.ThrowIfNull(value, paramName);
        if (value.Count == 0)
            throw new ArgumentException("Collection cannot be empty.", paramName);
        return value;
    }

    [DoesNotReturn]
    public static void Fail(string message) => throw new InvalidOperationException(message);
}