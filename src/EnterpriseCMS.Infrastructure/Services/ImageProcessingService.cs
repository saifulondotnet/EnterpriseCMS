using EnterpriseCMS.Core.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace EnterpriseCMS.Infrastructure.Services;

public record ImageVariants(
    string Original,
    string Large,
    string Medium,
    string Thumbnail,
    string WebP
);

public class ImageProcessingService
{
    private readonly IStorageService _storage;
    private readonly string _basePath;

    public ImageProcessingService(IStorageService storage, Microsoft.Extensions.Configuration.IConfiguration config)
    {
        _storage = storage;
        _basePath = config["Storage:LocalPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
    }

    public async Task<ImageVariants> ProcessImageAsync(string filePath, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, filePath.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(fullPath)) throw new FileNotFoundException(fullPath);

        using var image = await Image.LoadAsync(fullPath, ct);
        var dir = Path.GetDirectoryName(filePath)!;
        var stem = Path.GetFileNameWithoutExtension(filePath);
        var ext = Path.GetExtension(filePath);

        var large = await SaveVariantAsync(image, dir, $"{stem}-large{ext}", 1920, 0, ct);
        var medium = await SaveVariantAsync(image, dir, $"{stem}-medium{ext}", 800, 0, ct);
        var thumb = await SaveVariantAsync(image, dir, $"{stem}-thumbnail{ext}", 300, 300, ct);
        var webp = await SaveWebPAsync(image, dir, $"{stem}.webp", 1200, ct);

        return new ImageVariants(filePath, large, medium, thumb, webp);
    }

    private async Task<string> SaveVariantAsync(Image src, string dir, string name, int w, int h, CancellationToken ct)
    {
        using var clone = src.Clone(ctx => {
            if (h > 0) ctx.Resize(new ResizeOptions { Size = new Size(w, h), Mode = ResizeMode.Crop });
            else ctx.Resize(new ResizeOptions { Size = new Size(w, 0), Mode = ResizeMode.Max });
        });
        var relPath = $"{dir}/{name}";
        var fullPath = Path.Combine(_basePath, relPath.Replace('/', Path.DirectorySeparatorChar));
        await clone.SaveAsync(fullPath, cancellationToken: ct);
        return relPath;
    }

    private async Task<string> SaveWebPAsync(Image src, string dir, string name, int width, CancellationToken ct)
    {
        using var clone = src.Clone(ctx => ctx.Resize(new ResizeOptions { Size = new Size(width, 0), Mode = ResizeMode.Max }));
        var relPath = $"{dir}/{name}";
        var fullPath = Path.Combine(_basePath, relPath.Replace('/', Path.DirectorySeparatorChar));
        await clone.SaveAsync(fullPath, new WebpEncoder(), ct);
        return relPath;
    }
}
