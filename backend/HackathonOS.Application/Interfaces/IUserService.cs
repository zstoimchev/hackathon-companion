using HackathonOS.Application.DTOs;
using HackathonOS.Domain;

namespace HackathonOS.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<Paginated<UserResponse>> GetAllAsync(int pageNumber = 0, int pageSize = 100, CancellationToken ct = default);
    Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserResponse?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<UserResponse> UpdateAsync(Guid id, CreateUserRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}