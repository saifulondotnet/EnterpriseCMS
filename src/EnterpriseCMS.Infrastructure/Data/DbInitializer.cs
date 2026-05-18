using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EnterpriseCMS.Infrastructure.Data;

/// <summary>
/// Applies pending EF Core migrations and orchestrates seed data on startup.
/// </summary>
public sealed class DbInitializer
{
    private readonly CmsDbContext _context;
    private readonly DbSeeder _seeder;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(CmsDbContext context, DbSeeder seeder, ILogger<DbInitializer> logger)
    {
        _context = context;
        _seeder  = seeder;
        _logger  = logger;
    }

    public async Task InitialiseAsync(CancellationToken cancellationToken = default)
    {
        var pending = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pending.Any())
        {
            _logger.LogInformation("Applying {Count} pending migration(s)…", pending.Count());
            await _context.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Migrations applied.");
        }

        await _seeder.SeedAsync(cancellationToken);
    }
}
