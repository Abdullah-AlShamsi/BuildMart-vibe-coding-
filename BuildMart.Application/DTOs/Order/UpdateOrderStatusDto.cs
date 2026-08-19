namespace BuildMart.Application.DTOs.Order;

/// <summary>One of: Pending, Confirmed, Processing, Shipped, Delivered, Cancelled.</summary>
public class UpdateOrderStatusDto
{
    public string OrderStatus { get; set; } = string.Empty;
}
