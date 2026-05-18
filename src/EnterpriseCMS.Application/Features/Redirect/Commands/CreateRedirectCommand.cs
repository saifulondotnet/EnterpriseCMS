using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Redirect.Commands;

public record CreateRedirectCommand(string FromSlug, string ToSlug, bool IsRegex, int StatusCode) : IRequest<RedirectDto>;
