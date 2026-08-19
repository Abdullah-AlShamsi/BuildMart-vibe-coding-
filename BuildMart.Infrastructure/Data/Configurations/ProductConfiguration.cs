using BuildMart.Domain.Entities;
using BuildMart.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildMart.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.DiscountPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        builder.Property(p => p.Brand)
            .HasMaxLength(100);

        builder.Property(p => p.Weight)
            .HasColumnType("decimal(10,3)");

        builder.Property(p => p.Unit)
            .HasConversion<string>()
            .HasMaxLength(20);

        // SKU must be unique across the whole catalog.
        builder.HasIndex(p => p.SKU)
            .IsUnique();

        // Supports the common query patterns: filter by category,
        // filter by brand, filter/sort by price, and full catalog scans
        // for available items only.
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.Brand);
        builder.HasIndex(p => p.Price);
        builder.HasIndex(p => p.IsAvailable);
        builder.HasIndex(p => p.Name);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // EffectivePrice is computed in C# only, never persisted.
        builder.Ignore(p => p.EffectivePrice);

        builder.HasData(ProductSeedData.Products);
    }
}
