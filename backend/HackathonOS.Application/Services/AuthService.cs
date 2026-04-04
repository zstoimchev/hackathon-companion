using HackathonOS.Application.DTOs;
using HackathonOS.Application.Interfaces;
using HackathonOS.Repositories;

namespace HackathonOS.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IJwtService jwtService) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtService _jwtService = jwtService;

    public Task<AuthResponse> LoginAsync(AuthRequest authRequest, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task LogoutAsync(string token, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}