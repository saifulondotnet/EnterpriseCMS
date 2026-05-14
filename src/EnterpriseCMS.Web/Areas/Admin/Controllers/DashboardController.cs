using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin,Administrator,Editor,Author")]
public class DashboardController : Controller
{
    private readonly EnterpriseCMS.Core.Interfaces.IUnitOfWork _uow;

    public DashboardController(EnterpriseCMS.Core.Interfaces.IUnitOfWork uow) => _uow = uow;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var contentCount = await _uow.Contents.CountAsync(ct: ct);
        var mediaCount = await _uow.MediaAssets.CountAsync(ct: ct);
        ViewBag.ContentCount = contentCount;
        ViewBag.MediaCount = mediaCount;
        return View();
    }
}
