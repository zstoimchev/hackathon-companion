using HackathonOS.Application.DTOs.Auth;
using HackathonOS.Application.Interfaces;
using HackathonOS.Application.Services;
using HackathonOS.Domain.Entities;
using HackathonOS.Domain.Enums;

namespace HackathonOS.Services;

public class AuthService(
    IUserRepository userRepository, 
    IJwtService jwtService) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email, ct))
            throw new InvalidOperationException("Email already registered.");

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
            throw new ArgumentException($"Invalid role: {request.Role}");

        var user = new User
        {
            Email = request.Email.ToLowerInvariant(),
            Name = request.Name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = role
        };

        await userRepository.AddAsync(user, ct);
        await userRepository.SaveChangesAsync(ct);

        var token = jwtService.GenerateToken(user.Id, user.Email, user.Role.ToString());
        return new AuthResponse(token, user.Id, user.Email, user.Name, user.Role.ToString());
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByEmailAsync(request.Email.ToLowerInvariant(), ct)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is disabled.");

        var token = jwtService.GenerateToken(user.Id, user.Email, user.Role.ToString());
        return new AuthResponse(token, user.Id, user.Email, user.Name, user.Role.ToString());
    }
}
