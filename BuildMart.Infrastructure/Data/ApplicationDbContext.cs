using BuildMart.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BuildMart.Infrastructure.Data;

/// <summary>
/// EF Core context. Inherits from IdentityDbContext&lt;ApplicationUser&gt; so
/// Identity's own tables (AspNetUsers, AspNetRoles, AspNetUserRoles, ...)
/// are created and managed alongside BuildMart's domain tables.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Cart> Carts => Set<Cart>();

    public DbSet<CartItem> CartItems => Set<CartItem>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Must run first: configures all AspNetXxx Identity tables.
        base.OnModelCreating(modelBuilder);

        // Applies every IEntityTypeConfiguration<T> found in this assembly
        // (see Data/Configurations/*.cs) instead of configuring inline here.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        UpdateAuditTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Automatically stamps CreatedAt/UpdatedAt on every entity that
    /// derives from BaseEntity, so individual services never have to
    /// remember to do it manually.
    /// </summary>
    private void UpdateAuditTimestamps()
    {
        var entries = ChangeTracker.Entries<Domain.Common.BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }
}
