using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public record PublishContentCommand(Guid Id) : IRequest<ContentDto>;
