using HackathonOS.Application.Interfaces;
using HackathonOS.Domain.Entities;
using HackathonOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HackathonOS.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _db;

    public Repository(AppDbContext db) => _db = db;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Set<T>().FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        => await _db.Set<T>().ToListAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _db.Set<T>().AddAsync(entity, ct);

    public void Update(T entity)
        => _db.Set<T>().Update(entity);

    public void Remove(T entity)
        => _db.Set<T>().Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
