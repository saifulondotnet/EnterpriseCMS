using EnterpriseCMS.Application.Features.Auth.Commands;
using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnterpriseCMS.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class AccountController : Controller
{
    private readonly IMediator _mediator;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(IMediator mediator, SignInManager<ApplicationUser> signIn, UserManager<ApplicationUser> userManager)
    { _mediator = mediator; _signInManager = signIn; _userManager = userManager; }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(dto);
        try
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) { ModelState.AddModelError("", "Invalid credentials."); return View(dto); }
            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, dto.RememberMe, true);
            if (result.Succeeded) return LocalRedirect(returnUrl ?? "/Admin");
            if (result.IsLockedOut) { ModelState.AddModelError("", "Account is locked. Try again later."); return View(dto); }
            ModelState.AddModelError("", "Invalid credentials.");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet, AllowAnonymous]
    public IActionResult AccessDenied() => View();
}
