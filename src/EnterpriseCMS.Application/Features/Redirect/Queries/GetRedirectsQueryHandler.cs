using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Application.Features.Redirect.Commands;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Redirect.Queries;

public class GetRedirectsQueryHandler : IRequestHandler<GetRedirectsQuery, List<RedirectDto>>
{
    private readonly IUnitOfWork _uow;

    public GetRedirectsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<List<RedirectDto>> Handle(GetRedirectsQuery request, CancellationToken ct)
    {
        var redirects = await _uow.Redirects.GetAllAsync(ct);
        return redirects.OrderBy(r => r.FromSlug).Select(RedirectDtoMapper.MapToDto).ToList();
    }
}
