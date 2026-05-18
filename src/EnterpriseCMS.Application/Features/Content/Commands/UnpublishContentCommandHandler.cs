using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Enums;
using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public class UnpublishContentCommandHandler : IRequestHandler<UnpublishContentCommand, ContentDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public UnpublishContentCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<ContentDto> Handle(UnpublishContentCommand request, CancellationToken ct)
    {
        var content = await _uow.Contents.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Content", request.Id);

        content.Status = ContentStatus.Unpublished;
        content.UpdatedAt = DateTime.UtcNow;
        content.UpdatedBy = _currentUser.UserId;

        await _uow.Contents.UpdateAsync(content, ct);
        await _uow.SaveChangesAsync(ct);

        return ContentDtoMapper.MapToDto(content);
    }
}
