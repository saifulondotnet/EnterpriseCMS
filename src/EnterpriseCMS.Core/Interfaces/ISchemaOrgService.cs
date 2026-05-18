namespace EnterpriseCMS.Core.Interfaces;

public interface ISchemaOrgService
{
    string GenerateArticleSchema(string title, string description, string url, DateTime? publishedAt, string? authorName);
    string GenerateWebSiteSchema(string name, string url);
    string GenerateBreadcrumbSchema(IEnumerable<(string Name, string Url)> items);
}
