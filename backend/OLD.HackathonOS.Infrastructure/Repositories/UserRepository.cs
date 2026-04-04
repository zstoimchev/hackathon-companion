using HackathonOS.Application.Interfaces;
using HackathonOS.Domain.Entities;
using HackathonOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HackathonOS.Infrastructure.Repositories;

public class UserRepository : Repository<Userr>, IUserRepository
{
    public UserRepository(AppDbContext db) : base(db) { }

    public async Task<Userr?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => await _db.Users.AnyAsync(u => u.Email == email, ct);
}
