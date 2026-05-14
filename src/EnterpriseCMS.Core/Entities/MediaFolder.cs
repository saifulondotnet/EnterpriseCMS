namespace EnterpriseCMS.Core.Entities;

public class MediaFolder : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }
    public MediaFolder? Parent { get; set; }
    public ICollection<MediaFolder> Children { get; set; } = new List<MediaFolder>();
    public ICollection<MediaAsset> Assets { get; set; } = new List<MediaAsset>();
}
