using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BuildMart.Infrastructure.Data;

/// <summary>
/// Lets the EF Core CLI create an ApplicationDbContext at design time
/// (for `dotnet ef migrations add`) without needing to spin up the full
/// API host / DI container. Only used by tooling — never by the running app.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Falls back to a local SQL Server Express instance if no
        // BUILDMART_CONNECTION_STRING environment variable is set.
        // The real app instead reads BuildMart.API/appsettings*.json (see Program.cs).
        var connectionString = Environment.GetEnvironmentVariable("BUILDMART_CONNECTION_STRING")
            ?? "Server=.\\SQLEXPRESS;Database=BuildMartDb_Dev;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sql =>
            sql.MigrationsAssembly(typeof(ApplicationDbContextFactory).Assembly.FullName));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
