using EnterpriseCMS.Application.Common.Models;
using MediatR;

namespace EnterpriseCMS.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password, string? RecaptchaToken = null)
    : IRequest<AuthResponseDto>;
