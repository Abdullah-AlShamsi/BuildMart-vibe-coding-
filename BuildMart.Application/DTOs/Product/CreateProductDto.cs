namespace BuildMart.Application.DTOs.Product;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string? Brand { get; set; }
    public string Unit { get; set; } = "Piece";
    public decimal? Weight { get; set; }
    public bool IsAvailable { get; set; } = true;
}
