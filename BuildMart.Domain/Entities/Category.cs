using BuildMart.Domain.Common;

namespace BuildMart.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
