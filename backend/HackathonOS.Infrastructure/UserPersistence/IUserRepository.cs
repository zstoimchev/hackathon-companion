using HackathonOS.Domain;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Infrastructure.UserPersistence;

public interface IUserRepository
{
    Task<User?> CreateAsync(User request, CancellationToken ct = default);
    Task<Paginated<User>> GetAllAsync(int pageNumber = 0, int pageSize = 100, CancellationToken ct = default);
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> UpdateAsync(int id, User request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}