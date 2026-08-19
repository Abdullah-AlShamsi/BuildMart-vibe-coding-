using BuildMart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildMart.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.ShippingAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(o => o.PaymentStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(o => o.OrderStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Supports "my orders" queries and admin filtering/reporting by status and date.
        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.OrderStatus);
        builder.HasIndex(o => o.OrderDate);

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict); // preserve order history even if a user account is later deactivated

        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
