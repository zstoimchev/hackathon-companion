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

        var existing = await userRepository.GetUserDetailsAsync(request.Email, ct);
        if (existing != null) return result.CreateConflict("User with that email already exists.");

        var userModel = mapper.Map<User>(request);
        userModel.PasswordHash = hashingService.Hash(request.Password);

        var createdUser = await userRepository.CreateUserAsync(userModel, ct);
        _logger.LogInformation("Created new user: {@User}", createdUser);
        return createdUser != null
            ? result.CreateSuccess(mapper.Map<UserResponse>(createdUser))
            : result.CreateConflict("Failed to create user.");
    }

    public async Task<GdgResult<Paginated<UserResponse>>> GetAllUsersAsync(
        int pageNumber = 0,
        int pageSize = 100,
        CancellationToken ct = default)
    {
        var result = await userRepository.GetAllUsersAsync(pageNumber, pageSize, ct);
        var mapped = mapper.Map<Paginated<UserResponse>>(result);
        return new GdgResult<Paginated<UserResponse>>().CreateSuccess(mapped);
    }

    public async Task<GdgResult<UserResponse>> GetUserDetailsAsync(
        Guid guid,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetUserDetailsAsync(guid, ct);
        return user is null
            ? new GdgResult<UserResponse>().CreateNotFound()
            : new GdgResult<UserResponse>().CreateSuccess(mapper.Map<UserResponse>(user));
    }

    public async Task<GdgResult<UserResponse>> UpdateUserAsync(
        Guid id,
        UserRequest request,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
        var user = await userRepository.GetUserDetailsAsync(id, ct);
        if (user == null) return new GdgResult<UserResponse>().CreateNotFound();
        // TODO: here update all original value in user from request if they are present in request.
    }

    public async Task<GdgResult<UserResponse>> DeleteUserAsync(
        Guid id,
        CancellationToken ct = default)
    {
        var user = await userRepository.GetUserDetailsAsync(id, ct);
        if (user == null) return new GdgResult<UserResponse>().CreateNotFound();
        var deleted = await userRepository.DeleteUserAsync(id, ct);
        return !deleted
            ? new GdgResult<UserResponse>().CreateConflict("Failed to delete user.") 
            : new GdgResult<UserResponse>().CreateSuccess(mapper.Map<UserResponse>(user));
    }
}