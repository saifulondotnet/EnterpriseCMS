using EnterpriseCMS.Core.Enums;

namespace EnterpriseCMS.Core.Entities;

public class Content : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? Body { get; set; }
    public string Blocks { get; set; } = "[]";
    public string ContentType { get; set; } = "page";
    public ContentStatus Status { get; set; } = ContentStatus.Draft;
    public DateTime? PublishedAt { get; set; }
    public Guid? AuthorId { get; set; }
    public Guid? FeaturedImageId { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public int ViewCount { get; set; }
    public int SortOrder { get; set; }
    public Guid? ParentId { get; set; }
    public int CurrentVersion { get; set; } = 1;
    public string? Template { get; set; }
    public ApplicationUser? Author { get; set; }
    public MediaAsset? FeaturedImage { get; set; }
    public Content? Parent { get; set; }
    public ICollection<Content> Children { get; set; } = new List<Content>();
    public ICollection<ContentVersion> Versions { get; set; } = new List<ContentVersion>();
    public ICollection<ContentMeta> Meta { get; set; } = new List<ContentMeta>();
    public ICollection<ContentTag> ContentTags { get; set; } = new List<ContentTag>();
    public ICollection<ContentCategory> ContentCategories { get; set; } = new List<ContentCategory>();
}
