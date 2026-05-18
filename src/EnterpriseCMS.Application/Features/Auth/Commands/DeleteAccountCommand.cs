using MediatR;

namespace EnterpriseCMS.Application.Features.Auth.Commands;

public record DeleteAccountCommand(Guid UserId) : IRequest<bool>;
