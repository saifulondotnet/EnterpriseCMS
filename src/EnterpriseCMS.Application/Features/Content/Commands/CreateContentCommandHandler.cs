using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public class CreateContentCommandHandler : IRequestHandler<CreateContentCommand, ContentDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly ISlugService _slugService;

    public CreateContentCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser, ISlugService slugService)
    {
        _uow = uow;
        _currentUser = currentUser;
        _slugService = slugService;
    }

    public async Task<ContentDto> Handle(CreateContentCommand request, CancellationToken ct)
    {
        var slug = string.IsNullOrWhiteSpace(request.Slug) ? request.Title : request.Slug;
        var finalSlug = await _slugService.GenerateUniqueAsync(slug,
            async s => (await _uow.Contents.FindAsync(c => c.Slug == s, ct)).Any());

        var content = new Core.Entities.Content
        {
            Title = request.Title,
            Slug = finalSlug,
            Excerpt = request.Excerpt,
            Body = request.Body,
            Blocks = request.Blocks ?? "[]",
            ContentType = request.ContentType,
            Status = request.Status,
            PublishedAt = request.PublishedAt,
            AuthorId = _currentUser.UserId,
            TenantId = _currentUser.TenantId ?? Guid.Empty,
            MetaTitle = request.MetaTitle,
            MetaDescription = request.MetaDescription,
            CurrentVersion = 1
        };

        await _uow.Contents.AddAsync(content, ct);
        await _uow.SaveChangesAsync(ct);

        return MapToDto(content);
    }

    private static ContentDto MapToDto(Core.Entities.Content c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        Slug = c.Slug,
        Excerpt = c.Excerpt,
        Body = c.Body,
        Blocks = c.Blocks,
        ContentType = c.ContentType,
        Status = c.Status,
        PublishedAt = c.PublishedAt,
        AuthorId = c.AuthorId,
        MetaTitle = c.MetaTitle,
        MetaDescription = c.MetaDescription,
        CurrentVersion = c.CurrentVersion,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
