using BuildMart.Domain.Entities;
using BuildMart.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildMart.Infrastructure.Data.Seed;

/// <summary>
/// Seeds ASP.NET Core Identity roles and the default development admin
/// account. Runs at application startup (see BuildMart.API/Program.cs,
/// Phase 4) — never as part of a migration, because it needs
/// UserManager/RoleManager to hash the password correctly.
/// </summary>
public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(IdentitySeeder));

        // 1. Ensure both roles exist.
        foreach (var roleName in new[] { nameof(UserRole.Admin), nameof(UserRole.Customer) })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2. Ensure the default admin account exists.
        // These credentials come from configuration (appsettings / user-secrets / env vars)
        // and are DEVELOPMENT-ONLY defaults — see the warning in Program.cs and the README.
        var adminEmail = configuration["SeedAdmin:Email"] ?? "admin@buildmart.com";
        var adminPassword = configuration["SeedAdmin:Password"] ?? "Admin123!";
        var adminFullName = configuration["SeedAdmin:FullName"] ?? "BuildMart Administrator";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin is null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = adminFullName,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, nameof(UserRole.Admin));
                logger.LogWarning(
                    "Seeded default admin account '{Email}'. THIS USES A DEVELOPMENT-ONLY PASSWORD — change it immediately in any non-development environment.",
                    adminEmail);
            }
            else
            {
                logger.LogError(
                    "Failed to seed default admin account: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
