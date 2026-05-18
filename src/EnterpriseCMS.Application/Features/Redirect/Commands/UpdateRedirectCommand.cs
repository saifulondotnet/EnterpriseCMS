using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Redirect.Commands;

public record UpdateRedirectCommand(Guid Id, string FromSlug, string ToSlug, bool IsRegex, int StatusCode) : IRequest<RedirectDto>;
