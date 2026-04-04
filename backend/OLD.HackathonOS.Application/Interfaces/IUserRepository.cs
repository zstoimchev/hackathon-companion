using HackathonOS.Domain.Entities;

namespace HackathonOS.Application.Interfaces;

public interface IUserRepository : IRepository<Userr>
{
    Task<Userr?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
}
