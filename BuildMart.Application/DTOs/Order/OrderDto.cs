namespace BuildMart.Application.DTOs.Order;

public class OrderDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string OrderStatus { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}
