using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Entities;
using EnterpriseCMS.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EnterpriseCMS.Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken ct)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DisplayName = $"{request.FirstName} {request.LastName}".Trim(),
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
            throw new Core.Exceptions.ValidationException(errors);
        }

        await _userManager.AddToRoleAsync(user, Core.Interfaces.RoleNames.Subscriber);

        var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(user);
        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                DisplayName = user.DisplayName,
                Roles = new List<string> { Core.Interfaces.RoleNames.Subscriber }
            }
        };
    }
}
