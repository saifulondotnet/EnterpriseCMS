using EnterpriseCMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin,Administrator")]
public class SettingsController : Controller
{
    private readonly IUnitOfWork _uow;

    public SettingsController(IUnitOfWork uow) => _uow = uow;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var settings = await _uow.Settings.GetAllAsync(ct);
        return View(settings.GroupBy(s => s.Group).ToDictionary(g => g.Key, g => g.ToList()));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Dictionary<string, string> values, CancellationToken ct)
    {
        foreach (var kv in values)
        {
            var existing = (await _uow.Settings.FindAsync(s => s.SettingKey == kv.Key, ct)).FirstOrDefault();
            if (existing != null) { existing.SettingValue = kv.Value; existing.UpdatedAt = DateTime.UtcNow; await _uow.Settings.UpdateAsync(existing, ct); }
        }
        await _uow.SaveChangesAsync(ct);
        TempData["Success"] = "Settings saved.";
        return RedirectToAction(nameof(Index));
    }
}
