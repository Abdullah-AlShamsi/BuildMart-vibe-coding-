using BuildMart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildMart.Infrastructure.Data.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Quantity)
            .IsRequired();

        // The same product cannot appear twice in one cart —
        // adding it again must update the existing row's quantity instead.
        builder.HasIndex(ci => new { ci.CartId, ci.ProductId })
            .IsUnique();

        builder.HasOne(ci => ci.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict); // keep cart history consistent even if a product is later removed via soft-delete
    }
}
