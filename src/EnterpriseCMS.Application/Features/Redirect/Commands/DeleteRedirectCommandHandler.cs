using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Redirect.Commands;

public class DeleteRedirectCommandHandler : IRequestHandler<DeleteRedirectCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public DeleteRedirectCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteRedirectCommand request, CancellationToken ct)
    {
        var redirect = await _uow.Redirects.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Redirect", request.Id);

        redirect.IsDeleted = true;
        redirect.DeletedAt = DateTime.UtcNow;
        redirect.DeletedBy = _currentUser.UserId;

        await _uow.Redirects.UpdateAsync(redirect, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
