using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Application.Features.Widget.Commands;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Widget.Queries;

public class GetAllWidgetsQueryHandler : IRequestHandler<GetAllWidgetsQuery, List<WidgetDto>>
{
    private readonly IUnitOfWork _uow;

    public GetAllWidgetsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<List<WidgetDto>> Handle(GetAllWidgetsQuery request, CancellationToken ct)
    {
        var widgets = await _uow.Widgets.GetAllAsync(ct);
        return widgets.OrderBy(w => w.AreaSlug).ThenBy(w => w.Order)
            .Select(CreateWidgetCommandHandler.MapToDto).ToList();
    }
}
