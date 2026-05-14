using EnterpriseCMS.Application.Features.Content.Queries;
using EnterpriseCMS.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCMS.Web.Controllers;

public class HomeController : Controller
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var featured = await _mediator.Send(new GetContentsQuery(1, 6, null, ContentStatus.Published, "post"), ct);
        return View(featured);
    }

    [Route("/{slug}")]
    public async Task<IActionResult> Page(string slug, CancellationToken ct)
    {
        var results = await _mediator.Send(new GetContentsQuery(1, 1, slug, ContentStatus.Published), ct);
        var content = results.Items.FirstOrDefault(c => c.Slug == slug);
        if (content == null) return NotFound();
        return View("Page", content);
    }
}
