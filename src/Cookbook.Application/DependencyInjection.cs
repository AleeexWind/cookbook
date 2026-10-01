using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Application;

/// <summary>
/// Dependency injection helpers for the application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers MediatR handlers from the application assembly.
    /// The host must also call <c>AddLogging()</c> before resolving MediatR.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
