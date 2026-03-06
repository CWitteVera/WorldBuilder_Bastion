using BastionBuilder.Application.Interfaces;
using BastionBuilder.Persistence.Sqlite.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BastionBuilder.Persistence.Sqlite;

/// <summary>
/// Extension methods for registering SQLite persistence services.
/// </summary>
public static class PersistenceServiceExtensions
{
    /// <summary>
    /// Registers <see cref="BastionDbContext"/> (SQLite) and <see cref="BastionRepository"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">SQLite connection string (e.g., <c>Data Source=bastion.db</c>).</param>
    public static IServiceCollection AddSqlitePersistence(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<BastionDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IBastionRepository, BastionRepository>();

        return services;
    }

    /// <summary>
    /// Ensures the database is created and all pending migrations are applied.
    /// </summary>
    public static async Task InitialiseDatabaseAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        BastionDbContext db = scope.ServiceProvider.GetRequiredService<BastionDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
