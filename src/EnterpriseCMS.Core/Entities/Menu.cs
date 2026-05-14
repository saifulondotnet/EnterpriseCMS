namespace EnterpriseCMS.Core.Entities;

public class Menu : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Location { get; set; } = "primary";
    public ICollection<MenuItem> Items { get; set; } = new List<MenuItem>();
}
