using EnterpriseCMS.Core.Entities;

namespace EnterpriseCMS.Core.Interfaces;

public interface ITokenService
{
    Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(ApplicationUser user);
    Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(string refreshToken);
}
