using HackathonOS.Application.Interfaces;
using HackathonOS.Domain.Entities;
using HackathonOS.Domain.Enums;
using HackathonOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HackathonOS.Infrastructure.Repositories;

public class MentorRequestRepository : Repository<MentorRequest>, IMentorRequestRepository
{
    public MentorRequestRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<MentorRequest>> GetByEventAsync(Guid eventId, CancellationToken ct = default)
        => await _db.MentorRequests
            .Include(mr => mr.Team)
            .Include(mr => mr.AssignedMentor)
            .Where(mr => mr.EventId == eventId)
            .OrderByDescending(mr => mr.Priority)
            .ThenBy(mr => mr.CreatedOnUtc)
            .ToListAsync(ct);

    public async Task<IEnumerable<MentorRequest>> GetWaitingAsync(Guid eventId, CancellationToken ct = default)
        => await _db.MentorRequests
            .Include(mr => mr.Team)
            .Where(mr => mr.EventId == eventId && mr.Status == MentorRequestStatus.Waiting)
            .OrderByDescending(mr => mr.Priority)
            .ThenBy(mr => mr.CreatedOnUtc)
            .ToListAsync(ct);

    public async Task<IEnumerable<MentorRequest>> GetByMentorAsync(Guid mentorId, CancellationToken ct = default)
        => await _db.MentorRequests
            .Include(mr => mr.Team)
            .Where(mr => mr.AssignedMentorId == mentorId)
            .OrderByDescending(mr => mr.AssignedAt)
            .ToListAsync(ct);
}
