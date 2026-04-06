using HackathonOS.Domain;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Infrastructure.UserPersistence;

public interface IUserRepository
{
    Task<User?> CreateUserAsync(User request, CancellationToken ct = default);
    Task<Paginated<User>> GetAllUsersAsync(int pageNumber = 0, int pageSize = 100, CancellationToken ct = default);
    Task<User?> GetUserDetailsAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetUserDetailsAsync(string email, CancellationToken ct = default);
    Task<User?> UpdateUserAsync(Guid id, User request, CancellationToken ct = default);
    Task<bool> DeleteUserAsync(Guid guid, CancellationToken ct = default);
}