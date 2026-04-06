namespace HackathonOS.Infrastructure;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // BASE ENTITY
        // =========================
        modelBuilder.Entity<BaseEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Guid)
                .IsRequired();

            entity.HasIndex(x => x.Guid)
                .IsUnique();
        });

        // =========================
        // USER
        // =========================
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.Property(x => x.PasswordHash)
                .IsRequired();

            entity.Property(x => x.Role)
                .HasConversion<int>();

            entity.Property(x => x.IsActive)
                .IsRequired();
        });

        // =========================
        // SOFT DELETE FILTER
        // =========================
        modelBuilder.Entity<User>()
            .HasQueryFilter(x => x.DeletedOnUtc == null);
    }

    // =========================
    // AUTO TIMESTAMPS
    // =========================
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedOnUtc = DateTime.UtcNow;

            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedOnUtc = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(ct);
    }
}