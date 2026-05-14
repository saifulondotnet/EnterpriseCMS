namespace EnterpriseCMS.Core.Entities;

public class ContentMeta : BaseEntity
{
    public Guid ContentId { get; set; }
    public string MetaKey { get; set; } = string.Empty;
    public string? MetaValue { get; set; }
    public string FieldType { get; set; } = "text";
    public Content Content { get; set; } = null!;
}
