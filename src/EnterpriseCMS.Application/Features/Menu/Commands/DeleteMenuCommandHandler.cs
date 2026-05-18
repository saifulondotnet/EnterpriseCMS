using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Menu.Commands;

public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public DeleteMenuCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteMenuCommand request, CancellationToken ct)
    {
        var menu = await _uow.Menus.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Menu", request.Id);

        // Soft-delete all items
        var items = await _uow.MenuItems.FindAsync(mi => mi.MenuId == request.Id, ct);
        foreach (var item in items)
        {
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            item.DeletedBy = _currentUser.UserId;
            await _uow.MenuItems.UpdateAsync(item, ct);
        }

        menu.IsDeleted = true;
        menu.DeletedAt = DateTime.UtcNow;
        menu.DeletedBy = _currentUser.UserId;
        await _uow.Menus.UpdateAsync(menu, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
