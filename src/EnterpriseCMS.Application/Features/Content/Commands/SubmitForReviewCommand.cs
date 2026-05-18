using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public record SubmitForReviewCommand(Guid Id) : IRequest<ContentDto>;
