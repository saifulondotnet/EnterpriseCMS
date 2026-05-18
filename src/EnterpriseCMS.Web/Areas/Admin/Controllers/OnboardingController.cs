using EnterpriseCMS.Core.Entities;
using EnterpriseCMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin,Administrator")]
public class OnboardingController : Controller
{
    private readonly IUnitOfWork _uow;

    public OnboardingController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public IActionResult Step1() { ViewBag.Step = 1; return View(); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Step1(string siteName, string siteUrl, CancellationToken ct)
    {
        await SaveSetting("site_name", siteName, ct);
        await SaveSetting("site_url", siteUrl, ct);
        return RedirectToAction("Step2");
    }

    [HttpGet]
    public IActionResult Step2() { ViewBag.Step = 2; return View(); }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Step2Post() => RedirectToAction("Step3");

    [HttpGet]
    public IActionResult Step3() { ViewBag.Step = 3; return View(); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Step3(string theme, CancellationToken ct)
    {
        await SaveSetting("active_theme", theme, ct);
        return RedirectToAction("Step4");
    }

    [HttpGet]
    public IActionResult Step4() { ViewBag.Step = 4; return View(); }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Step4Post() => RedirectToAction("Step5");

    [HttpGet]
    public IActionResult Step5() { ViewBag.Step = 5; return View(); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(CancellationToken ct)
    {
        await SaveSetting("onboarding_complete", "true", ct);
        TempData["Success"] = "Setup complete! Welcome to EnterpriseCMS.";
        return RedirectToAction("Index", "Dashboard");
    }

    private async Task SaveSetting(string key, string value, CancellationToken ct)
    {
        var existing = await _uow.Settings.FindAsync(s => s.SettingKey == key, ct);
        if (existing.Any())
        {
            var setting = existing.First();
            setting.SettingValue = value;
            await _uow.Settings.UpdateAsync(setting, ct);
        }
        else
        {
            await _uow.Settings.AddAsync(new Setting { SettingKey = key, SettingValue = value }, ct);
        }
        await _uow.SaveChangesAsync(ct);
    }
}
