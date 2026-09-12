using FluentValidation;
using MediatR;
using Accordly.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Accordly.Application;

/// <summary>Registers application services.</summary>
public static class ApplicationServiceExtensions
{
    /// <summary>Registers CQRS handlers and validation.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(assembly);
        return services;
    }
}
