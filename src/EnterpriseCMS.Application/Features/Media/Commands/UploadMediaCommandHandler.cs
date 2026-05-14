using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Enums;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Media.Commands;

public class UploadMediaCommandHandler : IRequestHandler<UploadMediaCommand, MediaAssetDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IStorageService _storage;
    private readonly ICurrentUserService _currentUser;

    public UploadMediaCommandHandler(IUnitOfWork uow, IStorageService storage, ICurrentUserService currentUser)
    { _uow = uow; _storage = storage; _currentUser = currentUser; }

    public async Task<MediaAssetDto> Handle(UploadMediaCommand request, CancellationToken ct)
    {
        var path = await _storage.SaveFileAsync(request.FileStream, request.FileName, request.ContentType, ct);
        var mediaType = DetermineMediaType(request.ContentType);

        var asset = new Core.Entities.MediaAsset
        {
            FileName = Path.GetFileName(path),
            OriginalFileName = request.FileName,
            MimeType = request.ContentType,
            FileSize = request.FileSize,
            FilePath = path,
            MediaType = mediaType,
            FolderId = request.FolderId,
            TenantId = _currentUser.TenantId ?? Guid.Empty,
            IsProcessed = false
        };

        await _uow.MediaAssets.AddAsync(asset, ct);
        await _uow.SaveChangesAsync(ct);

        return new MediaAssetDto
        {
            Id = asset.Id, FileName = asset.FileName, OriginalFileName = asset.OriginalFileName,
            MimeType = asset.MimeType, FileSize = asset.FileSize, FilePath = asset.FilePath,
            PublicUrl = _storage.GetPublicUrl(path), MediaType = asset.MediaType,
            FolderId = asset.FolderId, CreatedAt = asset.CreatedAt
        };
    }

    private static MediaType DetermineMediaType(string contentType) => contentType.Split('/')[0] switch
    {
        "image" => MediaType.Image,
        "video" => MediaType.Video,
        "audio" => MediaType.Audio,
        _ => contentType.Contains("pdf") || contentType.Contains("document") ? MediaType.Document : MediaType.Other
    };
}
