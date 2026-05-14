using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public class DeleteContentCommandHandler : IRequestHandler<DeleteContentCommand, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public DeleteContentCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    { _uow = uow; _currentUser = currentUser; }

    public async Task<bool> Handle(DeleteContentCommand request, CancellationToken ct)
    {
        var content = await _uow.Contents.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Core.Entities.Content), request.Id);

        content.IsDeleted = true;
        content.DeletedAt = DateTime.UtcNow;
        content.DeletedBy = _currentUser.UserId;
        await _uow.Contents.UpdateAsync(content, ct);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
