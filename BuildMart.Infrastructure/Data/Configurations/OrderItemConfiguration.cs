using BuildMart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildMart.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.Property(oi => oi.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(oi => oi.TotalPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(oi => oi.OrderId);

        builder.HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict); // never delete a product row if it appears in historical orders
    }
}
