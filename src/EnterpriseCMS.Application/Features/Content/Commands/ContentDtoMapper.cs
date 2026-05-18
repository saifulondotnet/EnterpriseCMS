using EnterpriseCMS.Application.Common.Models;

namespace EnterpriseCMS.Application.Features.Content.Commands;

internal static class ContentDtoMapper
{
    internal static ContentDto MapToDto(Core.Entities.Content c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        Slug = c.Slug,
        Excerpt = c.Excerpt,
        Body = c.Body,
        Blocks = c.Blocks,
        ContentType = c.ContentType,
        Status = c.Status,
        PublishedAt = c.PublishedAt,
        AuthorId = c.AuthorId,
        MetaTitle = c.MetaTitle,
        MetaDescription = c.MetaDescription,
        CurrentVersion = c.CurrentVersion,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
