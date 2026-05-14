using EnterpriseCMS.Core.Enums;

namespace EnterpriseCMS.Application.Common.Models;

public class MediaAssetDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? AltText { get; set; }
    public string? Title { get; set; }
    public Guid? FolderId { get; set; }
    public DateTime CreatedAt { get; set; }
}
