using FluentValidation;
using MediatR;

namespace YourApp.Application.Abstractions;

/// <summary>
/// Pipeline behavior: validates each command/query with FluentValidation before the handler runs.
/// Wire up with: services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var validatorsList = validators as IList<IValidator<TRequest>> ?? validators.ToList();
        if (validatorsList.Count == 0)
            return await next(ct);

        var context = new ValidationContext<TRequest>(request);
        var failures = validatorsList
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next(ct);

        throw new ValidationException(failures);
    }
}