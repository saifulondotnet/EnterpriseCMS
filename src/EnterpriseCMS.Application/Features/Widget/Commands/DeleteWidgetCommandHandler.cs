using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Widget.Commands;

public class DeleteWidgetCommandHandler : IRequestHandler<DeleteWidgetCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public DeleteWidgetCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteWidgetCommand request, CancellationToken ct)
    {
        var widget = await _uow.Widgets.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Widget", request.Id);

        widget.IsDeleted = true;
        widget.DeletedAt = DateTime.UtcNow;
        widget.DeletedBy = _currentUser.UserId;

        await _uow.Widgets.UpdateAsync(widget, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
