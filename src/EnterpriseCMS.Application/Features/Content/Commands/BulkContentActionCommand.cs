using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public record BulkContentActionCommand(List<Guid> ContentIds, string Action) : IRequest;
