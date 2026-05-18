namespace EnterpriseCMS.Core.Entities;

public class Widget : BaseEntity
{
    public string AreaSlug { get; set; } = string.Empty;
    public string WidgetType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Settings { get; set; } = "{}";
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
}
