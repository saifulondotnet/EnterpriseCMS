using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Enums;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public record CreateContentCommand(
    string Title,
    string? Slug,
    string? Excerpt,
    string? Body,
    string Blocks,
    string ContentType,
    ContentStatus Status,
    DateTime? PublishedAt,
    string? MetaTitle,
    string? MetaDescription,
    List<string>? Tags,
    List<Guid>? CategoryIds
) : IRequest<ContentDto>;
