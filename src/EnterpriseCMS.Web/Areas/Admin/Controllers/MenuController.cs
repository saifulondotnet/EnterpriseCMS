using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Application.Features.Menu.Commands;
using EnterpriseCMS.Application.Features.Menu.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin,Administrator,Editor")]
public class MenuController : Controller
{
    private readonly IMediator _mediator;

    public MenuController(IMediator mediator) => _mediator = mediator;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var menus = await _mediator.Send(new GetMenusQuery(), ct);
        return View(menus);
    }

    [HttpGet]
    public IActionResult Create() => View(new MenuDto());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MenuDto vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        var result = await _mediator.Send(new CreateMenuCommand(vm.Name, vm.Slug, vm.Location), ct);
        TempData["Success"] = "Menu created.";
        return RedirectToAction(nameof(Edit), new { id = result.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var menu = await _mediator.Send(new GetMenuByIdQuery(id), ct);
        if (menu is null) return NotFound();
        return View(menu);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, MenuDto vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        var result = await _mediator.Send(new UpdateMenuCommand(id, vm.Name, vm.Slug, vm.Location), ct);
        TempData["Success"] = "Menu updated.";
        return View(result);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin,Administrator")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteMenuCommand(id), ct);
        TempData["Success"] = "Menu deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveItems([FromBody] SaveMenuItemsRequest request, CancellationToken ct)
    {
        await _mediator.Send(new UpdateMenuItemsCommand(request.MenuId, request.Items), ct);
        return Json(new { success = true });
    }
}

public class SaveMenuItemsRequest
{
    public Guid MenuId { get; set; }
    public List<MenuItemInputDto> Items { get; set; } = new();
}
