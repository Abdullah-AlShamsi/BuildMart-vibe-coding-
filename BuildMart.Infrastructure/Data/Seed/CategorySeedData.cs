using BuildMart.Domain.Entities;

namespace BuildMart.Infrastructure.Data.Seed;

/// <summary>
/// Static, deterministic seed rows for EF Core's HasData mechanism.
/// IDs are fixed on purpose: HasData compares by key across migrations,
/// so they must never be left to auto-increment.
/// </summary>
public static class CategorySeedData
{
    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static List<Category> Categories { get; } = new()
    {
        new Category { Id = 1, Name = "Power Tools", Description = "Electric and battery-powered tools for professional and DIY work.", ImageUrl = "/images/categories/power-tools.jpg", CreatedAt = SeedDate },
        new Category { Id = 2, Name = "Hand Tools", Description = "Manual tools for everyday construction and repair tasks.", ImageUrl = "/images/categories/hand-tools.jpg", CreatedAt = SeedDate },
        new Category { Id = 3, Name = "Construction Materials", Description = "Cement, concrete mix, adhesives and waterproofing materials.", ImageUrl = "/images/categories/construction-materials.jpg", CreatedAt = SeedDate },
        new Category { Id = 4, Name = "Safety Equipment", Description = "Personal protective equipment for job sites.", ImageUrl = "/images/categories/safety-equipment.jpg", CreatedAt = SeedDate },
        new Category { Id = 5, Name = "Painting Tools", Description = "Rollers, brushes, trays and surface prep tools.", ImageUrl = "/images/categories/painting-tools.jpg", CreatedAt = SeedDate },
        new Category { Id = 6, Name = "Electrical Tools", Description = "Tools and accessories for electrical installation work.", ImageUrl = "/images/categories/electrical-tools.jpg", CreatedAt = SeedDate },
        new Category { Id = 7, Name = "Plumbing Tools", Description = "Tools and fittings for plumbing and pipework.", ImageUrl = "/images/categories/plumbing-tools.jpg", CreatedAt = SeedDate },
        new Category { Id = 8, Name = "Hardware", Description = "Fasteners, hinges and general hardware supplies.", ImageUrl = "/images/categories/hardware.jpg", CreatedAt = SeedDate },
    };
}
