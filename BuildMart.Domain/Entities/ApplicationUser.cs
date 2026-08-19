using Microsoft.AspNetCore.Identity;

namespace BuildMart.Domain.Entities;

/// <summary>
/// Extends ASP.NET Core Identity's IdentityUser with the extra profile
/// fields BuildMart needs. Uses the default string (GUID) primary key
/// that Identity generates, which keeps Identity's built-in
/// UserManager/SignInManager plumbing working out of the box.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string? City { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public Cart? Cart { get; set; }
}
