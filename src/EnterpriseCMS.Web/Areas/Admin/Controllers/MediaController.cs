using EnterpriseCMS.Application.Features.Media.Commands;
using EnterpriseCMS.Application.Features.Media.Queries;
using EnterpriseCMS.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin,Administrator,Editor,Author")]
public class MediaController : Controller
{
    private readonly IMediator _mediator;
    private readonly long _maxFileSize = 50 * 1024 * 1024; // 50MB

    public MediaController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index(int page = 1, Guid? folderId = null, MediaType? mediaType = null, string? search = null, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetMediaQuery(page, 30, folderId, mediaType, search), ct);
        ViewBag.FolderId = folderId;
        ViewBag.MediaType = mediaType;
        ViewBag.Search = search;
        return View(result);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile file, Guid? folderId, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return Json(new { success = false, error = "No file provided." });

        if (file.Length > _maxFileSize)
            return Json(new { success = false, error = "File size exceeds 50MB limit." });

        var allowedTypes = new[] { "image/", "video/", "audio/", "application/pdf",
            "application/msword", "application/vnd.openxmlformats" };
        if (!allowedTypes.Any(t => file.ContentType.StartsWith(t)))
            return Json(new { success = false, error = "File type not allowed." });

        using var stream = file.OpenReadStream();
        var result = await _mediator.Send(new UploadMediaCommand(
            stream, file.FileName, file.ContentType, file.Length, folderId), ct);

        return Json(new { success = true, asset = result });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Administrator")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var asset = await _mediator.Send(new EnterpriseCMS.Application.Features.Media.Queries.GetMediaQuery(), ct);
        TempData["Success"] = "Asset deleted.";
        return RedirectToAction(nameof(Index));
    }
}
