using BuildMart.Application.DTOs.Product;
using FluentValidation;

namespace BuildMart.Application.Validators;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    private static readonly string[] ValidUnits =
        { "Piece", "Kilogram", "Liter", "Meter", "Box", "Bag", "Set" };

    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SKU).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.DiscountPrice)
            .LessThan(x => x.Price)
            .When(x => x.DiscountPrice.HasValue);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.Unit).Must(u => ValidUnits.Contains(u));
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0).When(x => x.Weight.HasValue);
    }
}
