using HackathonOS.Domain;
using HackathonOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HackathonOS.Infrastructure.UserPersistence;

public class UserRepositoryEf(AppDbContext dbContext)
{
    public async Task<User?> CreateUserAsync(User request, CancellationToken ct = default)
    {
        await dbContext.Users.AddAsync(request, ct);
        await dbContext.SaveChangesAsync(ct);

        return request;
    }

    public async Task<Paginated<User>> GetAllUsersAsync(int pageNumber = 0, int pageSize = 100, CancellationToken ct = default)
    {
        var query = dbContext.Users.AsNoTracking();

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new Paginated<User>
        {
            Items = items,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<User?> GetUserDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(x => x.Guid == id, ct);
    }

    public async Task<User?> GetUserDetailsAsync(string email, CancellationToken ct = default)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == email, ct);
    }

    public async Task<User?> UpdateUserAsync(int id, User request, CancellationToken ct = default)
    {
        var existing = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (existing == null)
            throw new KeyNotFoundException("User not found");

        // map fields manually (or use AutoMapper)
        existing.FirstName = request.FirstName;
        existing.LastName = request.LastName;
        existing.Email = request.Email;
        existing.Role = request.Role;
        existing.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(ct);

        return existing;
    }

    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Guid == id, ct);

        if (existing == null)
            return false;

        dbContext.Users.Remove(existing);

        await dbContext.SaveChangesAsync(ct);

        return true;
    }
}