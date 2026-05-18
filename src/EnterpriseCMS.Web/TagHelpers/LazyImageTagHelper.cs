using Microsoft.AspNetCore.Razor.TagHelpers;

namespace EnterpriseCMS.Web.TagHelpers;

[HtmlTargetElement("img")]
public class LazyImageTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!output.Attributes.ContainsName("loading"))
            output.Attributes.SetAttribute("loading", "lazy");
    }
}
