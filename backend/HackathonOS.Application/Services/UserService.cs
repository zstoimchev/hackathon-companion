using AutoMapper;
using HackathonOS.Application.DTOs;
using HackathonOS.Application.Interfaces;
using HackathonOS.Domain;
using HackathonOS.Domain.Entities;
using HackathonOS.Infrastructure.UserPersistence;
using Microsoft.Extensions.Logging;

namespace HackathonOS.Application.Services;

public class UserService(
    ILoggerFactory loggerFactory,
    IUserRepository userRepository,
    IMapper mapper,
    IHashingService hashingService) : IUserService
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<UserService>();

    public async Task<GdgResult<UserResponse>> CreateUserAsync(
        UserRequest request,
        CancellationToken ct = default)
    {
        var result = new GdgResult<UserResponse>();
        if (request.Equals(null)) return result.CreateConflict("Received empty user request.");
        // first check if the email exists

        var userModel = mapper.Map<User>(request);
        userModel.PasswordHash = hashingService.Hash(request.Password);

        var createdUser = await userRepository.CreateAsync(userModel, ct);
        _logger.LogInformation("Created new user: {@User}", createdUser);
        return createdUser != null
            ? result.CreateSuccess(mapper.Map<UserResponse>(createdUser))
            : result.CreateConflict("Failed to create user.");
    }

    public Task<GdgResult<Paginated<UserResponse>>> GetAllUsersAsync(
        int pageNumber = 0,
        int pageSize = 100,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<GdgResult<UserResponse>> GetUserDetailsAsync(
        Guid id,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserResponse?> GetByEmailAsync(
        string email,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<GdgResult<UserResponse>> UpdateUserAsync(
        Guid id,
        UserRequest request,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<GdgResult<UserResponse>> DeleteUserAsync(
        Guid id,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}