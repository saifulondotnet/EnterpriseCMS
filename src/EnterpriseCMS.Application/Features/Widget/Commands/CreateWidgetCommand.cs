using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Widget.Commands;

public record CreateWidgetCommand(string AreaSlug, string WidgetType, string Title, string Settings, int Order) : IRequest<WidgetDto>;
