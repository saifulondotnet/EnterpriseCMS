using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Menu.Queries;

public record GetMenuByIdQuery(Guid Id) : IRequest<MenuDto?>;
