using EnterpriseCMS.Core.Interfaces;
using EnterpriseCMS.Infrastructure.Data;
using EnterpriseCMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EnterpriseCMS.Infrastructure.BackgroundJobs;

public class ImageProcessingJob
{
    private readonly CmsDbContext _db;
    private readonly ImageProcessingService _processor;

    public ImageProcessingJob(CmsDbContext db, ImageProcessingService processor)
    { _db = db; _processor = processor; }

    public async Task ProcessAsync(Guid assetId, CancellationToken ct = default)
    {
        var asset = await _db.MediaAssets.FindAsync(new object[] { assetId }, ct);
        if (asset == null || asset.IsProcessed) return;
        if (!asset.MimeType.StartsWith("image/")) { asset.IsProcessed = true; await _db.SaveChangesAsync(ct); return; }

        try
        {
            var variants = await _processor.ProcessImageAsync(asset.FilePath, ct);
            asset.Variants = JsonSerializer.Serialize(variants);
            asset.IsProcessed = true;
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            asset.IsProcessed = true;
            await _db.SaveChangesAsync(ct);
        }
    }
}
