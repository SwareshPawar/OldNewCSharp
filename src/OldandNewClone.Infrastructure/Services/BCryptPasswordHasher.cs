using Microsoft.AspNetCore.Identity;
using BCrypt.Net;

namespace OldandNewClone.Infrastructure.Services;

public class BCryptPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
{
    // Match Node.js bcryptjs implementation with 10 rounds
    private const int WorkFactor = 10;

    public string HashPassword(TUser user, string password)
    {
        // Use 10 rounds to match Node.js bcryptjs implementation
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
        try
        {
            bool isValid = BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
            return isValid ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
        catch
        {
            return PasswordVerificationResult.Failed;
        }
    }
}
