using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Entities;
using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EnterpriseCMS.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
            ?? throw new NotFoundException("User", request.Email);

        if (!user.IsActive || user.IsDeleted)
            throw new ForbiddenException("Account is not active.");

        var valid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!valid) throw new ForbiddenException("Invalid credentials.");

        var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(user);
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName ?? $"{user.FirstName} {user.LastName}".Trim(),
                AvatarUrl = user.AvatarUrl,
                Roles = roles.ToList()
            }
        };
    }
}
