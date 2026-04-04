using HackathonOS.Application.DTOs.Auth;
using HackathonOS.Application.Interfaces;
using HackathonOS.Domain.Entities;
using HackathonOS.Domain.Enums;

namespace HackathonOS.Application.Services;

public class AuthServicee
{
    private readonly IUserRepository _users;
    private readonly IJwtService _jwt;

    public AuthServicee(IUserRepository users, IJwtService jwt)
    {
        _users = users;
        _jwt = jwt;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (await _users.ExistsByEmailAsync(request.Email, ct))
            throw new InvalidOperationException("Email already registered.");

        if (!Enum.TryParse<UserRolee>(request.Role, ignoreCase: true, out var role))
            throw new ArgumentException($"Invalid role: {request.Role}");

        var user = new Userr
        {
            Email = request.Email.ToLowerInvariant(),
            FirstName = request.Name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Rolee = role
        };

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        var token = _jwt.GenerateToken(user.Guid, user.Email, user.Rolee.ToString());
        return new AuthResponse(token, user.Guid, user.Email, user.FirstName, user.Rolee.ToString());
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByEmailAsync(request.Email.ToLowerInvariant(), ct)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is disabled.");

        var token = _jwt.GenerateToken(user.Guid, user.Email, user.Rolee.ToString());
        return new AuthResponse(token, user.Guid, user.Email, user.FirstName, user.Rolee.ToString());
    }
}
