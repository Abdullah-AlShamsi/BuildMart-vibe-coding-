namespace BuildMart.Application.DTOs.Cart;

public class CartDto
{
    public int Id { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public int TotalItems { get; set; }
}
