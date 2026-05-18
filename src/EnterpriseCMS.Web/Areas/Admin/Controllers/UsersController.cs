using EnterpriseCMS.Application.Features.Auth.Commands;
using EnterpriseCMS.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseCMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin,Administrator")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMediator _mediator;

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMediator mediator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var query = _userManager.Users.Where(u => !u.IsDeleted);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.Email!.Contains(search) || (u.DisplayName != null && u.DisplayName.Contains(search)));
        var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
        ViewBag.Search = search;
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string email, string firstName, string lastName, string? displayName, string role, string password)
    {
        var user = new ApplicationUser
        {
            UserName = email, Email = email,
            FirstName = firstName, LastName = lastName,
            DisplayName = displayName ?? $"{firstName} {lastName}".Trim(),
            IsActive = true, CreatedAt = DateTime.UtcNow
        };
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, role);
            TempData["Success"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }
        foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description);
        ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();
        ViewBag.Roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        ViewBag.UserRoles = await _userManager.GetRolesAsync(user);
        return View(user);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, string firstName, string lastName, string? displayName, string role)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        user.FirstName = firstName;
        user.LastName = lastName;
        user.DisplayName = displayName ?? $"{firstName} {lastName}".Trim();
        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, role);

        TempData["Success"] = "User updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();
        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);
        TempData["Success"] = user.IsActive ? "User activated." : "User deactivated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteAccountCommand(id));
        TempData["Success"] = "User account deleted.";
        return RedirectToAction(nameof(Index));
    }
}
