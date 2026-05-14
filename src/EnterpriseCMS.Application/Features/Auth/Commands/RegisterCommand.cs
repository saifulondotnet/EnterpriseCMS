using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Auth.Commands;

public record RegisterCommand(string Email, string Password, string FirstName, string LastName)
    : IRequest<AuthResponseDto>;
