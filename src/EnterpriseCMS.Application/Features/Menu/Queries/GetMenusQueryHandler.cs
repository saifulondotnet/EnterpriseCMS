using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Menu.Queries;

public class GetMenusQueryHandler : IRequestHandler<GetMenusQuery, List<MenuDto>>
{
    private readonly IUnitOfWork _uow;

    public GetMenusQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<List<MenuDto>> Handle(GetMenusQuery request, CancellationToken ct)
    {
        var menus = await _uow.Menus.GetAllAsync(ct);
        return menus.Select(m => new MenuDto
        {
            Id = m.Id,
            Name = m.Name,
            Slug = m.Slug,
            Location = m.Location
        }).ToList();
    }
}
