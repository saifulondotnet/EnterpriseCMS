using System.Text;
using System.Text.RegularExpressions;
using EnterpriseCMS.Core.Interfaces;

namespace EnterpriseCMS.Infrastructure.Services;

public partial class SlugService : ISlugService
{
    public string Generate(string text)
    {
        var str = text.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        str = NonAsciiRegex().Replace(str, "");
        str = str.Normalize(NormalizationForm.FormC);
        str = WhitespaceRegex().Replace(str.Trim(), "-");
        str = NonAlphanumericRegex().Replace(str, "");
        str = MultiHyphenRegex().Replace(str, "-").Trim('-');
        return str.Length > 200 ? str[..200] : str;
    }

    public async Task<string> GenerateUniqueAsync(string text, Func<string, Task<bool>> existsAsync)
    {
        var slug = Generate(text);
        var candidate = slug;
        var i = 1;
        while (await existsAsync(candidate)) candidate = $"{slug}-{i++}";
        return candidate;
    }

    [GeneratedRegex(@"\p{Mn}")]
    private static partial Regex NonAsciiRegex();
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
    [GeneratedRegex(@"[^a-z0-9\-]")]
    private static partial Regex NonAlphanumericRegex();
    [GeneratedRegex(@"-+")]
    private static partial Regex MultiHyphenRegex();
}
