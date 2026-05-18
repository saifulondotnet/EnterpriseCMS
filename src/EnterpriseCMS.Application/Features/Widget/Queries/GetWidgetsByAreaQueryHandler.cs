using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Application.Features.Widget.Commands;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Widget.Queries;

public class GetWidgetsByAreaQueryHandler : IRequestHandler<GetWidgetsByAreaQuery, List<WidgetDto>>
{
    private readonly IUnitOfWork _uow;

    public GetWidgetsByAreaQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<List<WidgetDto>> Handle(GetWidgetsByAreaQuery request, CancellationToken ct)
    {
        var widgets = await _uow.Widgets.FindAsync(
            w => w.AreaSlug == request.AreaSlug && w.IsActive, ct);
        return widgets.OrderBy(w => w.Order).Select(CreateWidgetCommandHandler.MapToDto).ToList();
    }
}
