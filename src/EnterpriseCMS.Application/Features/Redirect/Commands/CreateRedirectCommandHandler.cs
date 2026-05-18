using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Redirect.Commands;

public class CreateRedirectCommandHandler : IRequestHandler<CreateRedirectCommand, RedirectDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CreateRedirectCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<RedirectDto> Handle(CreateRedirectCommand request, CancellationToken ct)
    {
        var redirect = new Core.Entities.Redirect
        {
            FromSlug = request.FromSlug,
            ToSlug = request.ToSlug,
            IsRegex = request.IsRegex,
            StatusCode = request.StatusCode,
            TenantId = _currentUser.TenantId ?? Guid.Empty,
            CreatedBy = _currentUser.UserId
        };

        await _uow.Redirects.AddAsync(redirect, ct);
        await _uow.SaveChangesAsync(ct);

        return RedirectDtoMapper.MapToDto(redirect);
    }
}
