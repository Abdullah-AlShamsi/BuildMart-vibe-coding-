using BuildMart.Application.DTOs.Order;
using FluentValidation;

namespace BuildMart.Application.Validators;

public class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDto>
{
    private static readonly string[] ValidStatuses =
        { "Pending", "Confirmed", "Processing", "Shipped", "Delivered", "Cancelled" };

    public UpdateOrderStatusDtoValidator()
    {
        RuleFor(x => x.OrderStatus).Must(s => ValidStatuses.Contains(s))
            .WithMessage($"Order status must be one of: {string.Join(", ", ValidStatuses)}.");
    }
}
