namespace HackathonOS.Application.Services;

public interface IJwtService
{
    string GenerateToken(Guid userId, string email, string role);
    bool ValidateToken(string token, out Guid userId);
}
