using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace YourApp.Application.DependencyInjection;

public static class ApplicationExtensions
{
    /// <summary>
    /// Registers MediatR + FluentValidation + ValidationBehavior for the given assemblies.
    /// Usage: services.AddYourAppApplication(typeof(SomeHandler).Assembly);
    /// </summary>
    public static IServiceCollection AddYourAppApplication(
        this IServiceCollection services,
        params System.Reflection.Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        if (assemblies is null || assemblies.Length == 0)
            throw new ArgumentException("At least one assembly must be provided.", nameof(assemblies));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            cfg.AddOpenBehavior(typeof(Abstractions.ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true);
        return services;
    }
}