using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Enums;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Queries;

public record GetContentsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    ContentStatus? Status = null,
    string? ContentType = null,
    Guid? AuthorId = null
) : IRequest<PagedResult<ContentDto>>;
