using EnterpriseCMS.Core.Enums;
using EnterpriseCMS.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace EnterpriseCMS.Infrastructure.BackgroundJobs;

public class ScheduledContentJob
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<ScheduledContentJob> _logger;

    public ScheduledContentJob(IUnitOfWork uow, ILogger<ScheduledContentJob> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var now = DateTime.UtcNow;
        var scheduled = await _uow.Contents.FindAsync(
            c => c.Status == ContentStatus.Scheduled && c.PublishedAt <= now);

        foreach (var c in scheduled)
        {
            c.Status = ContentStatus.Published;
            await _uow.Contents.UpdateAsync(c);
        }

        if (scheduled.Any())
        {
            await _uow.SaveChangesAsync();
            _logger.LogInformation("Published {Count} scheduled items", scheduled.Count);
        }
    }
}
