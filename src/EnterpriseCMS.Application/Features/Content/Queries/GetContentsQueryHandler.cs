using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseCMS.Application.Features.Content.Queries;

public class GetContentsQueryHandler : IRequestHandler<GetContentsQuery, PagedResult<ContentDto>>
{
    private readonly IUnitOfWork _uow;

    public GetContentsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<PagedResult<ContentDto>> Handle(GetContentsQuery request, CancellationToken ct)
    {
        var query = _uow.Contents.Query();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c => c.Title.Contains(request.Search) || c.Slug.Contains(request.Search));
        if (request.Status.HasValue)
            query = query.Where(c => c.Status == request.Status.Value);
        if (!string.IsNullOrWhiteSpace(request.ContentType))
            query = query.Where(c => c.ContentType == request.ContentType);
        if (request.AuthorId.HasValue)
            query = query.Where(c => c.AuthorId == request.AuthorId.Value);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ContentDto
            {
                Id = c.Id, Title = c.Title, Slug = c.Slug, Excerpt = c.Excerpt,
                ContentType = c.ContentType, Status = c.Status, PublishedAt = c.PublishedAt,
                AuthorId = c.AuthorId, CurrentVersion = c.CurrentVersion,
                CreatedAt = c.CreatedAt, UpdatedAt = c.UpdatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<ContentDto>
        {
            Items = items, TotalCount = totalCount, Page = request.Page, PageSize = request.PageSize
        };
    }
}
