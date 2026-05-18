using EnterpriseCMS.Core.Enums;
using EnterpriseCMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseCMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin,Administrator,Editor,Author")]
public class DashboardController : Controller
{
    private readonly IUnitOfWork _uow;

    public DashboardController(IUnitOfWork uow) => _uow = uow;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var contentCount = await _uow.Contents.CountAsync(ct: ct);
        var mediaCount = await _uow.MediaAssets.CountAsync(ct: ct);
        var publishedCount = await _uow.Contents.CountAsync(c => c.Status == ContentStatus.Published, ct);
        var draftCount = await _uow.Contents.CountAsync(c => c.Status == ContentStatus.Draft, ct);

        var recentContent = await _uow.Contents.Query()
            .AsNoTracking()
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .Take(5)
            .Select(c => new { c.Id, c.Title, c.Status, c.ContentType, Date = c.UpdatedAt ?? c.CreatedAt })
            .ToListAsync(ct);

        var totalMediaSize = await _uow.MediaAssets.Query()
            .AsNoTracking()
            .SumAsync(m => (long)m.FileSize, ct);

        var settingsCount = await _uow.Settings.CountAsync(ct: ct);

        ViewBag.ContentCount = contentCount;
        ViewBag.MediaCount = mediaCount;
        ViewBag.PublishedCount = publishedCount;
        ViewBag.DraftCount = draftCount;
        ViewBag.RecentContent = recentContent;
        ViewBag.TotalMediaSizeMb = Math.Round(totalMediaSize / 1048576.0, 2);
        ViewBag.SystemHealthy = true;

        var onboardingComplete = await _uow.Settings.FindAsync(s => s.SettingKey == "onboarding_complete", ct);
        if (!onboardingComplete.Any())
            return RedirectToAction("Step1", "Onboarding");

        return View();
    }
}
