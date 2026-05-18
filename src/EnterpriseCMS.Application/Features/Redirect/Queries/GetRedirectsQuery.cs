using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Redirect.Queries;

public record GetRedirectsQuery : IRequest<List<RedirectDto>>;
