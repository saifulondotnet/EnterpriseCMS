using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Menu.Commands;

public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, MenuDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public UpdateMenuCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<MenuDto> Handle(UpdateMenuCommand request, CancellationToken ct)
    {
        var menu = await _uow.Menus.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Menu", request.Id);

        menu.Name = request.Name;
        menu.Slug = request.Slug;
        menu.Location = request.Location;
        menu.UpdatedAt = DateTime.UtcNow;
        menu.UpdatedBy = _currentUser.UserId;

        await _uow.Menus.UpdateAsync(menu, ct);
        await _uow.SaveChangesAsync(ct);

        return new MenuDto { Id = menu.Id, Name = menu.Name, Slug = menu.Slug, Location = menu.Location };
    }
}
