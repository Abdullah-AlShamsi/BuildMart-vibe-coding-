using BuildMart.Application.DTOs.Product;
using FluentValidation;

namespace BuildMart.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    private static readonly string[] ValidUnits =
        { "Piece", "Kilogram", "Liter", "Meter", "Box", "Bag", "Set" };

    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SKU).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
        RuleFor(x => x.DiscountPrice)
            .LessThan(x => x.Price)
            .When(x => x.DiscountPrice.HasValue)
            .WithMessage("Discount price must be lower than the regular price.");
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");
        RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("A valid category is required.");
        RuleFor(x => x.Unit).Must(u => ValidUnits.Contains(u))
            .WithMessage($"Unit must be one of: {string.Join(", ", ValidUnits)}.");
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0).When(x => x.Weight.HasValue);
    }
}
