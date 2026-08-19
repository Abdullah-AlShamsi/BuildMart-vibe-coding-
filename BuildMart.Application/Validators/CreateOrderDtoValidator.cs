using BuildMart.Application.DTOs.Order;
using FluentValidation;

namespace BuildMart.Application.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    private static readonly string[] ValidMethods = { "CashOnDelivery", "CreditCard", "BankTransfer" };

    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.ShippingAddress).NotEmpty().MaximumLength(500);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.PaymentMethod).Must(m => ValidMethods.Contains(m))
            .WithMessage($"Payment method must be one of: {string.Join(", ", ValidMethods)}.");
    }
}
