using EnterpriseCMS.Application.Features.Widget.Commands;
using EnterpriseCMS.Application.Features.Widget.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin,Administrator,Editor")]
public class WidgetController : Controller
{
    private readonly IMediator _mediator;

    public WidgetController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var widgets = await _mediator.Send(new GetAllWidgetsQuery(), ct);
        var grouped = widgets.GroupBy(w => w.AreaSlug).ToDictionary(g => g.Key, g => g.ToList());
        return View(grouped);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save([FromBody] SaveWidgetsRequest request, CancellationToken ct)
    {
        foreach (var w in request.Widgets)
        {
            if (w.Id == Guid.Empty)
                await _mediator.Send(new CreateWidgetCommand(w.AreaSlug, w.WidgetType, w.Title, w.Settings, w.Order), ct);
            else
                await _mediator.Send(new UpdateWidgetCommand(w.Id, w.AreaSlug, w.WidgetType, w.Title, w.Settings, w.Order, w.IsActive), ct);
        }
        return Json(new { success = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteWidgetCommand(id), ct);
        TempData["Success"] = "Widget deleted.";
        return RedirectToAction(nameof(Index));
    }
}

public class SaveWidgetsRequest
{
    public List<WidgetSaveItem> Widgets { get; set; } = new();
}

public class WidgetSaveItem
{
    public Guid Id { get; set; }
    public string AreaSlug { get; set; } = string.Empty;
    public string WidgetType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Settings { get; set; } = "{}";
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
}
