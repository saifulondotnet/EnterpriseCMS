using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Menu.Commands;

public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, MenuDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CreateMenuCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<MenuDto> Handle(CreateMenuCommand request, CancellationToken ct)
    {
        var menu = new Core.Entities.Menu
        {
            Name = request.Name,
            Slug = request.Slug,
            Location = request.Location,
            TenantId = _currentUser.TenantId ?? Guid.Empty,
            CreatedBy = _currentUser.UserId
        };

        await _uow.Menus.AddAsync(menu, ct);
        await _uow.SaveChangesAsync(ct);

        return new MenuDto { Id = menu.Id, Name = menu.Name, Slug = menu.Slug, Location = menu.Location };
    }
}
