using HackathonOS.Application.DTOs;
using HackathonOS.Application.Interfaces;
using HackathonOS.Domain;
using HackathonOS.Infrastructure.UserPersistence;

namespace HackathonOS.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;


    public Task<GdgResult<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Paginated<UserResponse>> GetAllAsync(
        int pageNumber = 0,
        int pageSize = 100,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserResponse?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserResponse> UpdateAsync(Guid id, CreateUserRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}