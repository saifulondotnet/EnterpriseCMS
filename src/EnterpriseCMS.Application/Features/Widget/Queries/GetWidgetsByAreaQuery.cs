using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Widget.Queries;

public record GetWidgetsByAreaQuery(string AreaSlug) : IRequest<List<WidgetDto>>;
