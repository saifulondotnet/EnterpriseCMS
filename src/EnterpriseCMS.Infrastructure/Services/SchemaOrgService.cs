using System.Text.Json;
using EnterpriseCMS.Core.Interfaces;

namespace EnterpriseCMS.Infrastructure.Services;

public class SchemaOrgService : ISchemaOrgService
{
    public string GenerateArticleSchema(string title, string description, string url, DateTime? publishedAt, string? authorName)
    {
        var schema = new
        {
            @context = "https://schema.org",
            @type = "Article",
            headline = title,
            description,
            url,
            datePublished = publishedAt?.ToString("o"),
            author = authorName is not null ? new { @type = "Person", name = authorName } : null
        };
        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = false });
    }

    public string GenerateWebSiteSchema(string name, string url)
    {
        var schema = new
        {
            @context = "https://schema.org",
            @type = "WebSite",
            name,
            url
        };
        return JsonSerializer.Serialize(schema);
    }

    public string GenerateBreadcrumbSchema(IEnumerable<(string Name, string Url)> items)
    {
        var list = items.Select((item, i) => new
        {
            @type = "ListItem",
            position = i + 1,
            name = item.Name,
            item = item.Url
        }).ToArray();

        var schema = new
        {
            @context = "https://schema.org",
            @type = "BreadcrumbList",
            itemListElement = list
        };
        return JsonSerializer.Serialize(schema);
    }
}
