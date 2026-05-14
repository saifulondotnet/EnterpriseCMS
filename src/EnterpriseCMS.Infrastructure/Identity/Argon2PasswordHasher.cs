using EnterpriseCMS.Core.Entities;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Identity;

namespace EnterpriseCMS.Infrastructure.Identity;

public class Argon2PasswordHasher : IPasswordHasher<ApplicationUser>
{
    public string HashPassword(ApplicationUser user, string password)
        => Argon2.Hash(password);

    public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
        => Argon2.Verify(hashedPassword, providedPassword)
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
}
