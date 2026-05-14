using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Queries;

public class GetContentByIdQueryHandler : IRequestHandler<GetContentByIdQuery, ContentDto>
{
    private readonly IUnitOfWork _uow;

    public GetContentByIdQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ContentDto> Handle(GetContentByIdQuery request, CancellationToken ct)
    {
        var c = await _uow.Contents.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Core.Entities.Content), request.Id);

        return new ContentDto
        {
            Id = c.Id, Title = c.Title, Slug = c.Slug, Excerpt = c.Excerpt,
            Body = c.Body, Blocks = c.Blocks, ContentType = c.ContentType,
            Status = c.Status, PublishedAt = c.PublishedAt, AuthorId = c.AuthorId,
            MetaTitle = c.MetaTitle, MetaDescription = c.MetaDescription,
            CurrentVersion = c.CurrentVersion, CreatedAt = c.CreatedAt, UpdatedAt = c.UpdatedAt
        };
    }
}
