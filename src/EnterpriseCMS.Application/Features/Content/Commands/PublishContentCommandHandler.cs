using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Enums;
using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public class PublishContentCommandHandler : IRequestHandler<PublishContentCommand, ContentDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public PublishContentCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<ContentDto> Handle(PublishContentCommand request, CancellationToken ct)
    {
        var content = await _uow.Contents.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Content", request.Id);

        content.Status = ContentStatus.Published;
        content.PublishedAt = DateTime.UtcNow;
        content.UpdatedAt = DateTime.UtcNow;
        content.UpdatedBy = _currentUser.UserId;

        await _uow.Contents.UpdateAsync(content, ct);
        await _uow.SaveChangesAsync(ct);

        return ContentDtoMapper.MapToDto(content);
    }
}
