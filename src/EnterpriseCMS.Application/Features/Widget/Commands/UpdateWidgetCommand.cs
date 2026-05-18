using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Widget.Commands;

public record UpdateWidgetCommand(Guid Id, string AreaSlug, string WidgetType, string Title, string Settings, int Order, bool IsActive) : IRequest<WidgetDto>;
