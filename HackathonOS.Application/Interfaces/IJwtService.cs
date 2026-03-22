using System;

namespace HackathonOS.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid userId, string email, string role);
    bool ValidateToken(string token, out Guid userId);
}
