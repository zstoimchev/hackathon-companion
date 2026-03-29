using HackathonOS.Application.Interfaces;
using HackathonOS.Domain.Entities;
using HackathonOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HackathonOS.Infrastructure.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(AppDbContext db) : base(db) { }

    public async Task<Event?> GetWithDetailsAsync(Guid id, CancellationToken ct = default)
        => await _db.Events
            .Include(e => e.Teams)
            .Include(e => e.Criteria)
            .Include(e => e.EventJudges).ThenInclude(ej => ej.Judge)
            .Include(e => e.EventMentors).ThenInclude(em => em.Mentor)
            .Include(e => e.MentorRequests)
            .FirstOrDefaultAsync(e => e.Guid == id, ct);

    public async Task<IEnumerable<Event>> GetActiveEventsAsync(CancellationToken ct = default)
        => await _db.Events
            .Where(e => e.IsActive)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync(ct);

    public async Task<IEnumerable<EventJudge>> GetEventJudgesAsync(Guid eventId, CancellationToken ct = default)
        => await _db.EventJudges
            .Include(ej => ej.Judge)
            .Where(ej => ej.EventId == eventId)
            .ToListAsync(ct);

    public async Task<IEnumerable<EventMentor>> GetEventMentorsAsync(Guid eventId, CancellationToken ct = default)
        => await _db.EventMentors
            .Include(em => em.Mentor)
            .Where(em => em.EventId == eventId)
            .ToListAsync(ct);
}
