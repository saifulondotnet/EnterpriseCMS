using EnterpriseCMS.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EnterpriseCMS.Infrastructure.Services;

public class LocalStorageService : IStorageService
{
    private readonly string _basePath;
    private readonly string _baseUrl;

    public LocalStorageService(IConfiguration config)
    {
        _basePath = config["Storage:LocalPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        _baseUrl = config["Storage:BaseUrl"] ?? "/uploads";
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveFileAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        var ext = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid():N}{ext}";
        var yearMonth = DateTime.UtcNow.ToString("yyyy/MM");
        var dir = Path.Combine(_basePath, yearMonth);
        Directory.CreateDirectory(dir);
        var filePath = Path.Combine(dir, uniqueName);

        await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await stream.CopyToAsync(fs, ct);

        return $"{yearMonth}/{uniqueName}";
    }

    public Task DeleteFileAsync(string path, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, path.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public string GetPublicUrl(string path) => $"{_baseUrl}/{path}";
}
