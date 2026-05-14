using EnterpriseCMS.Core.Enums;

namespace EnterpriseCMS.Application.Common.Models;

public class ContentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? Body { get; set; }
    public string Blocks { get; set; } = "[]";
    public string ContentType { get; set; } = "page";
    public ContentStatus Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public Guid? AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public int CurrentVersion { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Categories { get; set; } = new();
}
