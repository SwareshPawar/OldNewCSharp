namespace OldandNewClone.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(string userId, string email, string role);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    string? GetUserIdFromToken(string token);
}
