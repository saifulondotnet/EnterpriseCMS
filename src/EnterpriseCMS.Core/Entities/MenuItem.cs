namespace EnterpriseCMS.Core.Entities;

public class MenuItem : BaseEntity
{
    public Guid MenuId { get; set; }
    public Guid? ParentId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Url { get; set; }
    public Guid? ContentId { get; set; }
    public string Target { get; set; } = "_self";
    public int SortOrder { get; set; }
    public string? CssClass { get; set; }
    public Menu Menu { get; set; } = null!;
    public MenuItem? Parent { get; set; }
    public ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();
}
