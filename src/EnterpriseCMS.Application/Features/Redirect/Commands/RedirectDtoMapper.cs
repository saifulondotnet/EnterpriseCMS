using EnterpriseCMS.Application.Common.Models;

namespace EnterpriseCMS.Application.Features.Redirect.Commands;

internal static class RedirectDtoMapper
{
    internal static RedirectDto MapToDto(Core.Entities.Redirect r) => new()
    {
        Id = r.Id,
        FromSlug = r.FromSlug,
        ToSlug = r.ToSlug,
        IsRegex = r.IsRegex,
        StatusCode = r.StatusCode,
        CreatedAt = r.CreatedAt
    };
}
