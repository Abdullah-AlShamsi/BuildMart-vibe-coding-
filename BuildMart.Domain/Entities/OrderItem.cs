using BuildMart.Domain.Common;

namespace BuildMart.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    /// <summary>
    /// Unit price captured at the moment of purchase, so later price
    /// changes on the Product never rewrite historical order totals.
    /// </summary>
    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
