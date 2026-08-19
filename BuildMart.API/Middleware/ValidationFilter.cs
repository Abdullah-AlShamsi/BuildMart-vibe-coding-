using BuildMart.Application.DTOs.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuildMart.API.Middleware;

/// <summary>
/// Runs before every action. For each argument that has a matching
/// IValidator&lt;T&gt; registered (see BuildMart.Application.DependencyInjection),
/// it validates the object and short-circuits with 400 + a uniform
/// ApiResponse envelope if validation fails — so controllers stay free
/// of manual validation calls.
/// </summary>
public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (_serviceProvider.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                context.Result = new BadRequestObjectResult(
                    ApiResponse<object>.FailureResponse("Validation failed.", errors));
                return;
            }
        }

        await next();
    }
}
