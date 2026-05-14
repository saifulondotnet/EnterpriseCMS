using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace EnterpriseCMS.Web.TagHelpers;

public class CmsBlock
{
    public string Type { get; set; } = "richtext";
    public JsonElement? Data { get; set; }
    public string? Id { get; set; }
    public string? CssClass { get; set; }
}

[HtmlTargetElement("cms-blocks")]
public class BlockRendererTagHelper : TagHelper
{
    [HtmlAttributeName("blocks-json")]
    public string? BlocksJson { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.Attributes.SetAttribute("class", "cms-blocks");

        if (string.IsNullOrWhiteSpace(BlocksJson) || BlocksJson == "[]")
        {
            output.Content.Clear();
            return;
        }

        try
        {
            var blocks = JsonSerializer.Deserialize<List<CmsBlock>>(BlocksJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<CmsBlock>();

            var sb = new StringBuilder();
            foreach (var block in blocks)
                sb.Append(RenderBlock(block));

            output.Content.SetHtmlContent(sb.ToString());
        }
        catch
        {
            output.Content.SetHtmlContent($"<div class=\"alert alert-warning\">Block rendering error.</div>");
        }
    }

    private static string RenderBlock(CmsBlock block)
    {
        var css = block.CssClass ?? "";
        return block.Type switch
        {
            "richtext"  => $"<div class=\"cms-block cms-richtext {css}\">{block.Data?.GetString() ?? ""}</div>",
            "heading"   => $"<div class=\"cms-block cms-heading {css}\"><h{GetHeadingLevel(block)}>{GetDataString(block,"text")}</h{GetHeadingLevel(block)}></div>",
            "image"     => $"<div class=\"cms-block cms-image {css}\"><img src=\"{GetDataString(block,"url")}\" alt=\"{GetDataString(block,"alt")}\" class=\"img-fluid\" /></div>",
            "gallery"   => RenderGallery(block, css),
            "video"     => $"<div class=\"cms-block cms-video ratio ratio-16x9 {css}\"><iframe src=\"{GetDataString(block,"url")}\" allowfullscreen></iframe></div>",
            "cta"       => $"<div class=\"cms-block cms-cta text-center py-4 {css}\"><h3>{GetDataString(block,"heading")}</h3><p>{GetDataString(block,"subtext")}</p><a href=\"{GetDataString(block,"url")}\" class=\"btn btn-primary\">{GetDataString(block,"label")}</a></div>",
            "columns"   => $"<div class=\"cms-block cms-columns row {css}\">{GetDataString(block,"html")}</div>",
            "separator" => $"<div class=\"cms-block {css}\"><hr /></div>",
            "code"      => $"<div class=\"cms-block cms-code {css}\"><pre><code>{System.Web.HttpUtility.HtmlEncode(GetDataString(block,"code"))}</code></pre></div>",
            _           => $"<div class=\"cms-block {css}\">{block.Data?.ToString() ?? ""}</div>"
        };
    }

    private static int GetHeadingLevel(CmsBlock b)
    {
        if (b.Data.HasValue && b.Data.Value.ValueKind == JsonValueKind.Object
            && b.Data.Value.TryGetProperty("level", out var lv))
            return Math.Clamp(lv.GetInt32(), 1, 6);
        return 2;
    }

    private static string GetDataString(CmsBlock b, string key)
    {
        if (!b.Data.HasValue || b.Data.Value.ValueKind != JsonValueKind.Object) return "";
        if (b.Data.Value.TryGetProperty(key, out var v)) return v.GetString() ?? "";
        return "";
    }

    private static string RenderGallery(CmsBlock block, string css)
    {
        var items = new StringBuilder();
        if (block.Data.HasValue && block.Data.Value.TryGetProperty("images", out var imgs) && imgs.ValueKind == JsonValueKind.Array)
            foreach (var img in imgs.EnumerateArray())
                items.Append($"<div class=\"col-md-4 mb-3\"><img src=\"{img.GetString()}\" class=\"img-fluid rounded\" /></div>");
        return $"<div class=\"cms-block cms-gallery row {css}\">{items}</div>";
    }
}
