using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public record DeleteContentCommand(Guid Id) : IRequest<bool>;
