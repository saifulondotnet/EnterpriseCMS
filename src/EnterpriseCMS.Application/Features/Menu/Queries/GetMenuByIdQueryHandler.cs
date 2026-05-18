using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Menu.Queries;

public class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQuery, MenuDto?>
{
    private readonly IUnitOfWork _uow;

    public GetMenuByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<MenuDto?> Handle(GetMenuByIdQuery request, CancellationToken ct)
    {
        var menu = await _uow.Menus.GetByIdAsync(request.Id, ct);
        if (menu is null) return null;

        var allItems = await _uow.MenuItems.FindAsync(mi => mi.MenuId == request.Id, ct);

        var dto = new MenuDto
        {
            Id = menu.Id,
            Name = menu.Name,
            Slug = menu.Slug,
            Location = menu.Location,
            Items = BuildTree(allItems.ToList(), null)
        };

        return dto;
    }

    private static List<MenuItemDto> BuildTree(List<Core.Entities.MenuItem> all, Guid? parentId)
    {
        return all
            .Where(m => m.ParentId == parentId)
            .OrderBy(m => m.SortOrder)
            .Select(m => new MenuItemDto
            {
                Id = m.Id,
                MenuId = m.MenuId,
                ParentId = m.ParentId,
                Label = m.Label,
                Url = m.Url,
                Target = m.Target,
                SortOrder = m.SortOrder,
                CssClass = m.CssClass,
                Children = BuildTree(all, m.Id)
            }).ToList();
    }
}
