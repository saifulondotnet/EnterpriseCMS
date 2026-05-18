using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public record UnpublishContentCommand(Guid Id) : IRequest<ContentDto>;
