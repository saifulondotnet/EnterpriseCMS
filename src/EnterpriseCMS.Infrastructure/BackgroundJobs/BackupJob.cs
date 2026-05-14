using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EnterpriseCMS.Infrastructure.BackgroundJobs;

public class BackupJob
{
    private readonly IConfiguration _config;
    private readonly ILogger<BackupJob> _logger;

    public BackupJob(IConfiguration config, ILogger<BackupJob> logger)
    { _config = config; _logger = logger; }

    public async Task RunAsync(CancellationToken ct = default)
    {
        var backupDir = _config["Backup:Directory"] ?? "/backups";
        Directory.CreateDirectory(backupDir);
        var dbName = _config["Backup:DatabaseName"] ?? "EnterpriseCMS";
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var backupPath = Path.Combine(backupDir, $"{dbName}_{timestamp}.bak");
        var connStr = _config.GetConnectionString("DefaultConnection");

        try
        {
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync(ct);
            var sql = $"BACKUP DATABASE [{dbName}] TO DISK = N'{backupPath}' WITH NOFORMAT, INIT, COMPRESSION, STATS = 10";
            await using var cmd = new SqlCommand(sql, conn);
            cmd.CommandTimeout = 600;
            await cmd.ExecuteNonQueryAsync(ct);
            _logger.LogInformation("Database backup completed: {Path}", backupPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database backup failed");
        }
    }
}
