using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Entities;
using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Menu.Commands;

public class UpdateMenuItemsCommandHandler : IRequestHandler<UpdateMenuItemsCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public UpdateMenuItemsCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateMenuItemsCommand request, CancellationToken ct)
    {
        var menu = await _uow.Menus.GetByIdAsync(request.MenuId, ct)
            ?? throw new NotFoundException("Menu", request.MenuId);

        // Delete existing items
        var existing = await _uow.MenuItems.FindAsync(mi => mi.MenuId == request.MenuId, ct);
        foreach (var item in existing)
        {
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            item.DeletedBy = _currentUser.UserId;
            await _uow.MenuItems.UpdateAsync(item, ct);
        }

        // Insert new tree
        await InsertItemsAsync(request.MenuId, request.Items, null, ct);
        await _uow.SaveChangesAsync(ct);
    }

    private async Task InsertItemsAsync(Guid menuId, List<MenuItemInputDto> items, Guid? parentId, CancellationToken ct)
    {
        foreach (var dto in items)
        {
            var item = new MenuItem
            {
                MenuId = menuId,
                ParentId = parentId,
                Label = dto.Label,
                Url = dto.Url,
                Target = dto.Target,
                SortOrder = dto.SortOrder,
                CssClass = dto.CssClass,
                TenantId = _currentUser.TenantId ?? Guid.Empty,
                CreatedBy = _currentUser.UserId
            };
            await _uow.MenuItems.AddAsync(item, ct);
            if (dto.Children?.Count > 0)
                await InsertItemsAsync(menuId, dto.Children, item.Id, ct);
        }
    }
}
