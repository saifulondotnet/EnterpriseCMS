using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseCMS.Application.Features.Media.Queries;

public class GetMediaQueryHandler : IRequestHandler<GetMediaQuery, PagedResult<MediaAssetDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IStorageService _storage;

    public GetMediaQueryHandler(IUnitOfWork uow, IStorageService storage) { _uow = uow; _storage = storage; }

    public async Task<PagedResult<MediaAssetDto>> Handle(GetMediaQuery request, CancellationToken ct)
    {
        var query = _uow.MediaAssets.Query().AsNoTracking();
        if (request.FolderId.HasValue) query = query.Where(m => m.FolderId == request.FolderId);
        if (request.MediaType.HasValue) query = query.Where(m => m.MediaType == request.MediaType);
        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(m => m.OriginalFileName.Contains(request.Search) || (m.Title != null && m.Title.Contains(request.Search)));

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(m => m.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(m => new MediaAssetDto
            {
                Id = m.Id, FileName = m.FileName, OriginalFileName = m.OriginalFileName,
                MimeType = m.MimeType, FileSize = m.FileSize, FilePath = m.FilePath,
                PublicUrl = _storage.GetPublicUrl(m.FilePath), MediaType = m.MediaType,
                Width = m.Width, Height = m.Height, AltText = m.AltText, Title = m.Title,
                FolderId = m.FolderId, CreatedAt = m.CreatedAt
            }).ToListAsync(ct);

        return new PagedResult<MediaAssetDto> { Items = items, TotalCount = total, Page = request.Page, PageSize = request.PageSize };
    }
}
