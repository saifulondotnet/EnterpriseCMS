using EnterpriseCMS.Core.Enums;
using EnterpriseCMS.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public class BulkContentActionCommandHandler : IRequestHandler<BulkContentActionCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<BulkContentActionCommandHandler> _logger;

    public BulkContentActionCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser, ILogger<BulkContentActionCommandHandler> logger)
    {
        _uow = uow;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task Handle(BulkContentActionCommand request, CancellationToken ct)
    {
        foreach (var id in request.ContentIds)
        {
            var content = await _uow.Contents.GetByIdAsync(id, ct);
            if (content is null) continue;

            switch (request.Action.ToLowerInvariant())
            {
                case "publish":
                    content.Status = ContentStatus.Published;
                    content.PublishedAt ??= DateTime.UtcNow;
                    break;
                case "unpublish":
                    content.Status = ContentStatus.Unpublished;
                    break;
                case "archive":
                    content.Status = ContentStatus.Archived;
                    break;
                case "delete":
                    content.IsDeleted = true;
                    content.DeletedAt = DateTime.UtcNow;
                    content.DeletedBy = _currentUser.UserId;
                    break;
                default:
                    _logger.LogWarning("Unknown bulk action: {Action}", request.Action);
                    continue;
            }

            content.UpdatedAt = DateTime.UtcNow;
            content.UpdatedBy = _currentUser.UserId;
            await _uow.Contents.UpdateAsync(content, ct);
        }

        await _uow.SaveChangesAsync(ct);
    }
}
