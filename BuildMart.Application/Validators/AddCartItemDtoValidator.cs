using BuildMart.Application.DTOs.Cart;
using FluentValidation;

namespace BuildMart.Application.Validators;

public class AddCartItemDtoValidator : AbstractValidator<AddCartItemDto>
{
    public AddCartItemDtoValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1.");
    }
}
