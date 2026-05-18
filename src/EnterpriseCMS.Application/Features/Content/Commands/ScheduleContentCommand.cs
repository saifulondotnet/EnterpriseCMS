using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public record ScheduleContentCommand(Guid Id, DateTime PublishAt) : IRequest<ContentDto>;
