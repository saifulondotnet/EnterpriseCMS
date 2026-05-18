using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Menu.Commands;

public record UpdateMenuItemsCommand(Guid MenuId, List<MenuItemInputDto> Items) : IRequest;
