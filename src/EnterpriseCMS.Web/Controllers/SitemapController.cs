using EnterpriseCMS.Application.Features.Content.Queries;
using EnterpriseCMS.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Linq;

namespace EnterpriseCMS.Web.Controllers;

[Route("sitemap.xml")]
public class SitemapController : Controller
{
    private readonly IMediator _mediator;

    public SitemapController(IMediator mediator) => _mediator = mediator;

    [ResponseCache(Duration = 3600)]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var contents = await _mediator.Send(
            new GetContentsQuery(1, 1000, null, ContentStatus.Published, null), ct);

        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var ns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
        var urlset = new XElement(ns + "urlset",
            contents.Items.Select(c => new XElement(ns + "url",
                new XElement(ns + "loc", $"{baseUrl}/{c.Slug}"),
                new XElement(ns + "lastmod", (c.UpdatedAt ?? c.CreatedAt).ToString("yyyy-MM-dd")),
                new XElement(ns + "changefreq", "weekly"),
                new XElement(ns + "priority", "0.7")
            ))
        );

        var doc = new XDocument(new XDeclaration("1.0", "utf-8", null), urlset);
        var xml = doc.ToString();

        return Content(xml, "application/xml", Encoding.UTF8);
    }
}
