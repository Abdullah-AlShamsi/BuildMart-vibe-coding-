using BuildMart.Application.DTOs.Cart;
using FluentValidation;

namespace BuildMart.Application.Validators;

public class UpdateCartItemDtoValidator : AbstractValidator<UpdateCartItemDto>
{
    public UpdateCartItemDtoValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1.");
    }
}
