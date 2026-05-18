using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Menu.Commands;

public record UpdateMenuCommand(Guid Id, string Name, string Slug, string Location) : IRequest<MenuDto>;
