using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Application.Features.Redirect.Commands;
using EnterpriseCMS.Application.Features.Redirect.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin,Administrator")]
public class RedirectController : Controller
{
    private readonly IMediator _mediator;

    public RedirectController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var redirects = await _mediator.Send(new GetRedirectsQuery(), ct);
        return View(redirects);
    }

    [HttpGet]
    public IActionResult Create() => View(new RedirectDto());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RedirectViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        await _mediator.Send(new CreateRedirectCommand(vm.FromSlug, vm.ToSlug, vm.IsRegex, vm.StatusCode), ct);
        TempData["Success"] = "Redirect created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var redirects = await _mediator.Send(new GetRedirectsQuery(), ct);
        var redirect = redirects.FirstOrDefault(r => r.Id == id);
        if (redirect is null) return NotFound();
        return View(new RedirectViewModel { FromSlug = redirect.FromSlug, ToSlug = redirect.ToSlug, IsRegex = redirect.IsRegex, StatusCode = redirect.StatusCode });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, RedirectViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        await _mediator.Send(new UpdateRedirectCommand(id, vm.FromSlug, vm.ToSlug, vm.IsRegex, vm.StatusCode), ct);
        TempData["Success"] = "Redirect updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteRedirectCommand(id), ct);
        TempData["Success"] = "Redirect deleted.";
        return RedirectToAction(nameof(Index));
    }
}

public class RedirectViewModel
{
    public string FromSlug { get; set; } = string.Empty;
    public string ToSlug { get; set; } = string.Empty;
    public bool IsRegex { get; set; }
    public int StatusCode { get; set; } = 301;
}
