using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Application.Features.Content.Commands;
using EnterpriseCMS.Application.Features.Content.Queries;
using EnterpriseCMS.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin,Administrator,Editor,Author")]
public class ContentController : Controller
{
    private readonly IMediator _mediator;

    public ContentController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index(int page = 1, string? search = null, ContentStatus? status = null, string contentType = "page", CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetContentsQuery(page, 20, search, status, contentType), ct);
        ViewBag.Search = search;
        ViewBag.Status = status;
        ViewBag.ContentType = contentType;
        return View(result);
    }

    [HttpGet]
    public IActionResult Create(string contentType = "page")
    {
        ViewBag.ContentType = contentType;
        return View(new ContentDto { ContentType = contentType });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateContentViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        var result = await _mediator.Send(new CreateContentCommand(
            vm.Title, vm.Slug, vm.Excerpt, vm.Body, vm.Blocks ?? "[]",
            vm.ContentType, vm.Status, vm.PublishedAt, vm.MetaTitle, vm.MetaDescription, null, null), ct);
        TempData["Success"] = "Content created successfully.";
        return RedirectToAction(nameof(Edit), new { id = result.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var content = await _mediator.Send(new GetContentByIdQuery(id), ct);
        return View(content);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateContentViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        var result = await _mediator.Send(new UpdateContentCommand(
            id, vm.Title, vm.Slug, vm.Excerpt, vm.Body, vm.Blocks ?? "[]",
            vm.Status, vm.PublishedAt, vm.MetaTitle, vm.MetaDescription, false), ct);
        TempData["Success"] = "Content saved successfully.";
        return View(result);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Administrator")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteContentCommand(id), ct);
        TempData["Success"] = "Content deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AutoSave(Guid id, UpdateContentViewModel vm, CancellationToken ct)
    {
        try
        {
            await _mediator.Send(new UpdateContentCommand(
                id, vm.Title, vm.Slug, vm.Excerpt, vm.Body, vm.Blocks ?? "[]",
                vm.Status, vm.PublishedAt, vm.MetaTitle, vm.MetaDescription, true), ct);
            return Json(new { success = true, savedAt = DateTime.UtcNow });
        }
        catch
        {
            return Json(new { success = false });
        }
    }
}

public class CreateContentViewModel
{
    public string Title { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Excerpt { get; set; }
    public string? Body { get; set; }
    public string? Blocks { get; set; }
    public string ContentType { get; set; } = "page";
    public ContentStatus Status { get; set; } = ContentStatus.Draft;
    public DateTime? PublishedAt { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class UpdateContentViewModel : CreateContentViewModel { }
