using BuildMart.Application.Interfaces;
using BuildMart.Domain.Entities;
using BuildMart.Infrastructure.Data;
using BuildMart.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildMart.Infrastructure;

/// <summary>
/// Single entry point the API project calls (in Program.cs, Phase 4) to
/// wire up everything the Infrastructure layer provides: the DbContext,
/// Identity, and — starting in Phase 3 — repository implementations.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found. Set it in appsettings.json or via user-secrets/environment variables.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password policy — reasonable defaults for a store handling real orders.
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;

                options.User.RequireUniqueEmail = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ICustomerService, CustomerService>();

        return services;
    }
}
