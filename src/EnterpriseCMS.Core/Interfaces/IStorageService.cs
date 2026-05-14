namespace EnterpriseCMS.Core.Interfaces;

public interface IStorageService
{
    Task<string> SaveFileAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
    Task DeleteFileAsync(string path, CancellationToken ct = default);
    string GetPublicUrl(string path);
}
