using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Menu.Queries;

public record GetMenusQuery : IRequest<List<MenuDto>>;
