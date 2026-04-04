using HackathonOS.Domain;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Infrastructure.UserPersistence;

public class UserRepositoryClient : IUserRepository
{
    public Task<User> CreateAsync(User request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Paginated<User>> GetAllAsync(int pageNumber = 0, int pageSize = 100, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<User> UpdateAsync(Guid id, User request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}