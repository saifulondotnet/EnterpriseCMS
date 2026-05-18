using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Widget.Commands;

public class CreateWidgetCommandHandler : IRequestHandler<CreateWidgetCommand, WidgetDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CreateWidgetCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<WidgetDto> Handle(CreateWidgetCommand request, CancellationToken ct)
    {
        var widget = new Core.Entities.Widget
        {
            AreaSlug = request.AreaSlug,
            WidgetType = request.WidgetType,
            Title = request.Title,
            Settings = request.Settings,
            Order = request.Order,
            TenantId = _currentUser.TenantId ?? Guid.Empty,
            CreatedBy = _currentUser.UserId
        };

        await _uow.Widgets.AddAsync(widget, ct);
        await _uow.SaveChangesAsync(ct);

        return MapToDto(widget);
    }

    internal static WidgetDto MapToDto(Core.Entities.Widget w) => new()
    {
        Id = w.Id,
        AreaSlug = w.AreaSlug,
        WidgetType = w.WidgetType,
        Title = w.Title,
        Settings = w.Settings,
        Order = w.Order,
        IsActive = w.IsActive
    };
}
