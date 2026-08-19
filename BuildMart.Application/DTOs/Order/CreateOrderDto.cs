namespace BuildMart.Application.DTOs.Order;

/// <summary>
/// Orders are created from whatever is currently in the user's cart —
/// this DTO only carries the checkout-specific fields.
/// </summary>
public class CreateOrderDto
{
    public string ShippingAddress { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>One of: CashOnDelivery, CreditCard, BankTransfer.</summary>
    public string PaymentMethod { get; set; } = "CashOnDelivery";
}
