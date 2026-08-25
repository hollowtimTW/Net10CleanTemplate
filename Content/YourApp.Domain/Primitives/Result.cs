namespace YourApp.Domain.Primitives;

/// <summary>
/// Discriminated-union-like result type. Use for in-domain "expected failure" returns,
/// not for catastrophic errors. Use exceptions only for bugs and infrastructure failures.
/// </summary>
public readonly struct Result<T>
{
    private readonly T? _value;
    private readonly DomainError? _error;

    public bool IsSuccess => _error is null;
    public bool IsFailed => _error is not null;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException(
            $"Cannot read Value of a failed Result. Error: {_error}");

    public DomainError Error => IsFailed
        ? _error!
        : throw new InvalidOperationException(
            $"Cannot read Error of a successful Result. Value: {_value}");

    private Result(T value) { _value = value; _error = null; }
    private Result(DomainError error) { _value = default; _error = error; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(DomainError error) => new(error);

    public static implicit operator Result<T>(T value) => Success(value);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<DomainError, TResult> onFailure)
        => IsSuccess ? onSuccess(_value!) : onFailure(_error!);

    public void Switch(Action<T> onSuccess, Action<DomainError> onFailure)
    {
        if (IsSuccess) onSuccess(_value!);
        else onFailure(_error!);
    }
}

public static class Result
{
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(DomainError error) => Result<T>.Failure(error);
    public static Result<Unit> Success() => Result<Unit>.Success(Unit.Value);
    public static Result<Unit> Failure(DomainError error) => Result<Unit>.Failure(error);
}

public readonly record struct Unit
{
    public static readonly Unit Value;
}