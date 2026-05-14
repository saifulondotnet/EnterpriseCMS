namespace EnterpriseCMS.Core.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ICollection<ContentTag> ContentTags { get; set; } = new List<ContentTag>();
}
