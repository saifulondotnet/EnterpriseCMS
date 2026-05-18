using MediatR;

namespace EnterpriseCMS.Application.Features.Menu.Commands;

public record DeleteMenuCommand(Guid Id) : IRequest;
