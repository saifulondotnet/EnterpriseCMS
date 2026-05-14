namespace EnterpriseCMS.Core.Entities;

public class ContentCategory
{
    public Guid ContentId { get; set; }
    public Guid CategoryId { get; set; }
    public Content Content { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
