using EnterpriseCMS.Application.Common.Models;
using EnterpriseCMS.Core.Exceptions;
using EnterpriseCMS.Core.Interfaces;
using MediatR;

namespace EnterpriseCMS.Application.Features.Redirect.Commands;

public class UpdateRedirectCommandHandler : IRequestHandler<UpdateRedirectCommand, RedirectDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public UpdateRedirectCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<RedirectDto> Handle(UpdateRedirectCommand request, CancellationToken ct)
    {
        var redirect = await _uow.Redirects.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Redirect", request.Id);

        redirect.FromSlug = request.FromSlug;
        redirect.ToSlug = request.ToSlug;
        redirect.IsRegex = request.IsRegex;
        redirect.StatusCode = request.StatusCode;
        redirect.UpdatedAt = DateTime.UtcNow;
        redirect.UpdatedBy = _currentUser.UserId;

        await _uow.Redirects.UpdateAsync(redirect, ct);
        await _uow.SaveChangesAsync(ct);

        return RedirectDtoMapper.MapToDto(redirect);
    }
}
