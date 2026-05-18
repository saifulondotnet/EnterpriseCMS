using MediatR;

namespace EnterpriseCMS.Application.Features.Widget.Commands;

public record DeleteWidgetCommand(Guid Id) : IRequest;
