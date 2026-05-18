using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public record ArchiveContentCommand(Guid Id) : IRequest<ContentDto>;
