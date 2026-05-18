using EnterpriseCMS.Core.Entities;
using EnterpriseCMS.Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace EnterpriseCMS.Application.Features.Auth.Commands;

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteAccountCommandHandler(UserManager<ApplicationUser> userManager)
        => _userManager = userManager;

    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString())
            ?? throw new NotFoundException("User", request.UserId);

        user.Email = $"deleted_{user.Id}@deleted.invalid";
        user.UserName = $"deleted_{user.Id}@deleted.invalid";
        user.NormalizedEmail = $"DELETED_{user.Id}@DELETED.INVALID";
        user.NormalizedUserName = $"DELETED_{user.Id}@DELETED.INVALID";
        user.FirstName = null;
        user.LastName = null;
        user.DisplayName = null;
        user.IsDeleted = true;
        user.IsActive = false;
        user.RefreshToken = null;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}
