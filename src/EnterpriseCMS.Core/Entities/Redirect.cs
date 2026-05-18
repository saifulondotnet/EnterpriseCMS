namespace EnterpriseCMS.Core.Entities;

public class Redirect : BaseEntity
{
    public string FromSlug { get; set; } = string.Empty;
    public string ToSlug { get; set; } = string.Empty;
    public bool IsRegex { get; set; }
    public int StatusCode { get; set; } = 301;
}
