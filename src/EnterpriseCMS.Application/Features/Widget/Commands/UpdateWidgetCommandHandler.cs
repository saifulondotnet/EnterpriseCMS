using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Widget.Commands;

public class UpdateWidgetCommandHandler : IRequestHandler<UpdateWidgetCommand, WidgetDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public UpdateWidgetCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<WidgetDto> Handle(UpdateWidgetCommand request, CancellationToken ct)
    {
        var widget = await _uow.Widgets.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Widget", request.Id);

        widget.AreaSlug = request.AreaSlug;
        widget.WidgetType = request.WidgetType;
        widget.Title = request.Title;
        widget.Settings = request.Settings;
        widget.Order = request.Order;
        widget.IsActive = request.IsActive;
        widget.UpdatedAt = DateTime.UtcNow;
        widget.UpdatedBy = _currentUser.UserId;

        await _uow.Widgets.UpdateAsync(widget, ct);
        await _uow.SaveChangesAsync(ct);

        return CreateWidgetCommandHandler.MapToDto(widget);
    }
}
