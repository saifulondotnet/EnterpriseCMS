using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Enums;
using MediatR;

namespace EnterpriseCMS.Application.Features.Media.Queries;

public record GetMediaQuery(
    int Page = 1,
    int PageSize = 30,
    Guid? FolderId = null,
    MediaType? MediaType = null,
    string? Search = null
) : IRequest<PagedResult<MediaAssetDto>>;
