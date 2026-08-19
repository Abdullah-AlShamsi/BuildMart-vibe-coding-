using BuildMart.Domain.Common;
using BuildMart.Domain.Enums;

namespace BuildMart.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    /// <summary>
    /// Optional discounted price. When set and lower than Price,
    /// this is the price actually charged to the customer.
    /// </summary>
    public decimal? DiscountPrice { get; set; }

    public int StockQuantity { get; set; }

    /// <summary>
    /// Stock Keeping Unit — must be unique across the catalog.
    /// </summary>
    public string SKU { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }

    public string? Brand { get; set; }

    public MeasurementUnit Unit { get; set; } = MeasurementUnit.Piece;

    /// <summary>
    /// Weight in kilograms, used for shipping calculations.
    /// </summary>
    public decimal? Weight { get; set; }

    public bool IsAvailable { get; set; } = true;

    // Navigation properties
    public Category Category { get; set; } = null!;

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    /// <summary>
    /// The price actually applied at checkout: DiscountPrice when it is
    /// set and lower than Price, otherwise Price.
    /// </summary>
    public decimal EffectivePrice => DiscountPrice.HasValue && DiscountPrice.Value < Price
        ? DiscountPrice.Value
        : Price;
}
