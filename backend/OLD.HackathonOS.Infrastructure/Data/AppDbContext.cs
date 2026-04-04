using HackathonOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HackathonOS.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Userr> Users => Set<Userr>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventJudge> EventJudges => Set<EventJudge>();
    public DbSet<EventMentor> EventMentors => Set<EventMentor>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Criterion> Criteria => Set<Criterion>();
    public DbSet<Score> Scores => Set<Score>();
    public DbSet<MentorRequest> MentorRequests => Set<MentorRequest>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // User
        mb.Entity<Userr>(b =>
        {
            b.HasKey(u => u.Guid);
            b.HasIndex(u => u.Email).IsUnique();
            b.Property(u => u.Email).IsRequired().HasMaxLength(256);
            b.Property(u => u.FirstName).IsRequired().HasMaxLength(200);
            b.Property(u => u.PasswordHash).IsRequired();
            b.Property(u => u.Rolee).HasConversion<string>();
        });

        // Event
        mb.Entity<Event>(b =>
        {
            b.HasKey(e => e.Guid);
            b.Property(e => e.Name).IsRequired().HasMaxLength(200);
        });

        // EventJudge
        mb.Entity<EventJudge>(b =>
        {
            b.HasKey(ej => ej.Guid);
            b.HasIndex(ej => new { ej.EventId, ej.JudgeId }).IsUnique();
            b.HasOne(ej => ej.Event)
             .WithMany(e => e.EventJudges)
             .HasForeignKey(ej => ej.EventId)
             .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(ej => ej.Judge);
            // .WithMany(u => u.EventJudges)
            // .HasForeignKey(ej => ej.JudgeId)
            // .OnDelete(DeleteBehavior.Restrict);
        });

        // EventMentor
        mb.Entity<EventMentor>(b =>
        {
            b.HasKey(em => em.Guid);
            b.HasIndex(em => new { em.EventId, em.MentorId }).IsUnique();
            b.HasOne(em => em.Event)
             .WithMany(e => e.EventMentors)
             .HasForeignKey(em => em.EventId)
             .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(em => em.Mentor);
             // .WithMany(u => u.EventMentors)
             // .HasForeignKey(em => em.MentorId)
             // .OnDelete(DeleteBehavior.Restrict);
            b.Property(em => em.Expertise)
             .HasColumnType("text[]");
        });

        // Team
        mb.Entity<Team>(b =>
        {
            b.HasKey(t => t.Guid);
            b.Property(t => t.Name).IsRequired().HasMaxLength(200);
            b.HasOne(t => t.Event)
             .WithMany(e => e.Teams)
             .HasForeignKey(t => t.EventId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Criterion
        mb.Entity<Criterion>(b =>
        {
            b.HasKey(c => c.Guid);
            b.Property(c => c.Name).IsRequired().HasMaxLength(200);
            b.HasOne(c => c.Event)
             .WithMany(e => e.Criteria)
             .HasForeignKey(c => c.EventId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Score
        mb.Entity<Score>(b =>
        {
            b.HasKey(s => s.Guid);
            b.HasIndex(s => new { s.JudgeId, s.TeamId, s.CriterionId }).IsUnique();
            b.HasOne(s => s.Judge);
             // .WithMany(u => u.Scores)
             // .HasForeignKey(s => s.JudgeId)
             // .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(s => s.Team)
             .WithMany(t => t.Scores)
             .HasForeignKey(s => s.TeamId)
             .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(s => s.Criterion)
             .WithMany(c => c.Scores)
             .HasForeignKey(s => s.CriterionId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // MentorRequest
        mb.Entity<MentorRequest>(b =>
        {
            b.HasKey(mr => mr.Guid);
            b.Property(mr => mr.Topic).IsRequired().HasMaxLength(300);
            b.Property(mr => mr.Status).HasConversion<string>();
            b.HasOne(mr => mr.Event)
             .WithMany(e => e.MentorRequests)
             .HasForeignKey(mr => mr.EventId)
             .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(mr => mr.Team)
             .WithMany(t => t.MentorRequests)
             .HasForeignKey(mr => mr.TeamId)
             .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(mr => mr.AssignedMentor);
            // .WithMany(u => u.AssignedRequests)
            // .HasForeignKey(mr => mr.AssignedMentorId)
            // .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
