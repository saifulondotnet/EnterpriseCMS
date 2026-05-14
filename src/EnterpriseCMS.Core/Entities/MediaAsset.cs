using EnterpriseCMS.Core.Enums;

namespace EnterpriseCMS.Core.Entities;

public class MediaAsset : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? AltText { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid? FolderId { get; set; }
    public bool IsProcessed { get; set; }
    public string? Variants { get; set; }
    public MediaFolder? Folder { get; set; }
}
