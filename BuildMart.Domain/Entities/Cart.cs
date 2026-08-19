using BuildMart.Domain.Common;

namespace BuildMart.Domain.Entities;

/// <summary>
/// One cart per user. Created lazily the first time a user adds an item.
/// </summary>
public class Cart : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    // Navigation properties
    public ApplicationUser User { get; set; } = null!;

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
