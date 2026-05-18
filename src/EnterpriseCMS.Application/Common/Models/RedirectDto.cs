namespace EnterpriseCMS.Application.Common.Models;

public class RedirectDto
{
    public Guid Id { get; set; }
    public string FromSlug { get; set; } = string.Empty;
    public string ToSlug { get; set; } = string.Empty;
    public bool IsRegex { get; set; }
    public int StatusCode { get; set; } = 301;
    public DateTime CreatedAt { get; set; }
}
