namespace EnterpriseCMS.Application.Common.Models;

public class MenuDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Location { get; set; } = "primary";
    public List<MenuItemDto> Items { get; set; } = new();
}

public class MenuItemDto
{
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public Guid? ParentId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string Target { get; set; } = "_self";
    public int SortOrder { get; set; }
    public string? CssClass { get; set; }
    public List<MenuItemDto> Children { get; set; } = new();
}

public class MenuItemInputDto
{
    public string Label { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string Target { get; set; } = "_self";
    public int SortOrder { get; set; }
    public string? CssClass { get; set; }
    public List<MenuItemInputDto> Children { get; set; } = new();
}
