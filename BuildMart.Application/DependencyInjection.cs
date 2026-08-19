using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BuildMart.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registers every AbstractValidator<T> found in this assembly
        // (see Validators/*.cs) so ASP.NET Core's model binding pipeline
        // can invoke them automatically for matching DTOs.
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
