using EnterpriseCMS.Application.Features.Content.Queries;
using EnterpriseCMS.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace EnterpriseCMS.Web.Controllers;

public class HomeController : Controller
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator) => _mediator = mediator;

    [OutputCache(Duration = 300)]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var featured = await _mediator.Send(new GetContentsQuery(1, 6, null, ContentStatus.Published, "post"), ct);
        return View(featured);
    }

    [Route("/{slug}")]
    [OutputCache(Duration = 300)]
    public async Task<IActionResult> Page(string slug, CancellationToken ct)
    {
        var results = await _mediator.Send(new GetContentsQuery(1, 1, slug, ContentStatus.Published), ct);
        var content = results.Items.FirstOrDefault(c => c.Slug == slug);
        if (content == null) return NotFound();
        return View("Page", content);
    }

    [Route("/Home/Error404")]
    public IActionResult Error404() => View("~/Views/Shared/Error404.cshtml");

    [Route("/Home/Error500")]
    public IActionResult Error500() => View("~/Views/Shared/Error500.cshtml");

    [Route("/Home/Error")]
    public IActionResult Error() => View("~/Views/Shared/Error.cshtml");
}
