using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Enums;
using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public class DuplicateContentCommandHandler : IRequestHandler<DuplicateContentCommand, ContentDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly ISlugService _slugService;

    public DuplicateContentCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser, ISlugService slugService)
    {
        _uow = uow;
        _currentUser = currentUser;
        _slugService = slugService;
    }

    public async Task<ContentDto> Handle(DuplicateContentCommand request, CancellationToken ct)
    {
        var source = await _uow.Contents.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Content", request.Id);

        var newTitle = source.Title + " (Copy)";
        var baseSlug = source.Slug + "-copy";
        var finalSlug = await _slugService.GenerateUniqueAsync(baseSlug,
            async s => (await _uow.Contents.FindAsync(c => c.Slug == s, ct)).Any());

        var copy = new Core.Entities.Content
        {
            Title = newTitle,
            Slug = finalSlug,
            Excerpt = source.Excerpt,
            Body = source.Body,
            Blocks = source.Blocks,
            ContentType = source.ContentType,
            Status = ContentStatus.Draft,
            MetaTitle = source.MetaTitle,
            MetaDescription = source.MetaDescription,
            AuthorId = _currentUser.UserId,
            TenantId = _currentUser.TenantId ?? Guid.Empty,
            CurrentVersion = 1,
            CreatedBy = _currentUser.UserId
        };

        await _uow.Contents.AddAsync(copy, ct);
        await _uow.SaveChangesAsync(ct);

        return ContentDtoMapper.MapToDto(copy);
    }
}
