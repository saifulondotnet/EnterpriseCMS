namespace EnterpriseCMS.Core.Entities;

public class ContentVersion : BaseEntity
{
    public Guid ContentId { get; set; }
    public int VersionNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string Blocks { get; set; } = "[]";
    public string? ChangeNote { get; set; }
    public bool IsAutoSave { get; set; }
    public Content Content { get; set; } = null!;
}
