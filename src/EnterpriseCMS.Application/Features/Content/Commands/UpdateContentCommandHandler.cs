using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public class UpdateContentCommandHandler : IRequestHandler<UpdateContentCommand, ContentDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public UpdateContentCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<ContentDto> Handle(UpdateContentCommand request, CancellationToken ct)
    {
        var content = await _uow.Contents.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Core.Entities.Content), request.Id);

        // Save version before update
        var version = new Core.Entities.ContentVersion
        {
            ContentId = content.Id,
            VersionNumber = content.CurrentVersion,
            Title = content.Title,
            Body = content.Body,
            Blocks = content.Blocks,
            TenantId = content.TenantId,
            IsAutoSave = request.IsAutoSave
        };
        await _uow.ContentVersions.AddAsync(version, ct);

        content.Title = request.Title;
        content.Excerpt = request.Excerpt;
        content.Body = request.Body;
        content.Blocks = request.Blocks ?? "[]";
        content.Status = request.Status;
        content.PublishedAt = request.PublishedAt;
        content.MetaTitle = request.MetaTitle;
        content.MetaDescription = request.MetaDescription;
        content.UpdatedAt = DateTime.UtcNow;
        content.UpdatedBy = _currentUser.UserId;
        content.CurrentVersion++;

        await _uow.Contents.UpdateAsync(content, ct);
        await _uow.SaveChangesAsync(ct);

        return new ContentDto
        {
            Id = content.Id, Title = content.Title, Slug = content.Slug,
            Excerpt = content.Excerpt, Body = content.Body, Blocks = content.Blocks,
            ContentType = content.ContentType, Status = content.Status,
            PublishedAt = content.PublishedAt, AuthorId = content.AuthorId,
            MetaTitle = content.MetaTitle, MetaDescription = content.MetaDescription,
            CurrentVersion = content.CurrentVersion, CreatedAt = content.CreatedAt, UpdatedAt = content.UpdatedAt
        };
    }
}
