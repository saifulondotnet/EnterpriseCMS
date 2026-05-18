using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Widget.Queries;

public record GetAllWidgetsQuery : IRequest<List<WidgetDto>>;
