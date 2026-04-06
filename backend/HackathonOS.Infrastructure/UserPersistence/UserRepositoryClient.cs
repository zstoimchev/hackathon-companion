using HackathonOS.Domain;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Infrastructure.UserPersistence;

public class UserRepositoryClient : IUserRepository
{
    private const string ClientName = "client";
    
    public Task<User?> CreateUserAsync(User request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Paginated<User>> GetAllUsersAsync(int pageNumber = 0, int pageSize = 100, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserDetailsAsync(Guid id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserDetailsAsync(string email, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> UpdateUserAsync(int id, User request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUserAsync(Guid guid, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}