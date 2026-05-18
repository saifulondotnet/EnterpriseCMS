using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Menu.Commands;

public record CreateMenuCommand(string Name, string Slug, string Location) : IRequest<MenuDto>;
