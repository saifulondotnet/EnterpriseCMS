using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCMS.Web.Controllers;

[Route("robots.txt")]
public class RobotsController : Controller
{
    [ResponseCache(Duration = 3600)]
    public IActionResult Index()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var content = $"""
User-agent: *
Allow: /
Disallow: /Admin/
Disallow: /api/

Sitemap: {baseUrl}/sitemap.xml
""";
        return Content(content, "text/plain");
    }
}
