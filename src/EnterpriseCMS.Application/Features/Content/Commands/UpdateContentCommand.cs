using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Enums;
using MediatR;

namespace EnterpriseCMS.Application.Features.Content.Commands;

public record UpdateContentCommand(
    Guid Id,
    string Title,
    string? Slug,
    string? Excerpt,
    string? Body,
    string Blocks,
    ContentStatus Status,
    DateTime? PublishedAt,
    string? MetaTitle,
    string? MetaDescription,
    bool IsAutoSave = false
) : IRequest<ContentDto>;
