using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Queries;

public record GetContentByIdQuery(Guid Id) : IRequest<ContentDto>;
