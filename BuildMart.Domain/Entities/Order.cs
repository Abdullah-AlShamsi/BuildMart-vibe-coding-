using BuildMart.Domain.Common;
using BuildMart.Domain.Enums;

namespace BuildMart.Domain.Entities;

public class Order : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public PaymentMethod PaymentMethod { get; set; }

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    // Navigation properties
    public ApplicationUser User { get; set; } = null!;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
