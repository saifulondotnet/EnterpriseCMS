using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Media.Commands;

public record UploadMediaCommand(
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileSize,
    Guid? FolderId = null
) : IRequest<MediaAssetDto>;
