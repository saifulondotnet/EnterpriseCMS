using MediatR;

namespace EnterpriseCMS.Application.Features.Redirect.Commands;

public record DeleteRedirectCommand(Guid Id) : IRequest;
