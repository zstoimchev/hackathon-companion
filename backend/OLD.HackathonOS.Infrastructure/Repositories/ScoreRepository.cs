using HackathonOS.Application.Interfaces;
using HackathonOS.Domain.Entities;
using HackathonOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HackathonOS.Infrastructure.Repositories;

public class ScoreRepository : Repository<Score>, IScoreRepository
{
    public ScoreRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Score>> GetByEventAsync(Guid eventId, CancellationToken ct = default)
        => await _db.Scores
            .Include(s => s.Judge)
            .Include(s => s.Team)
            .Include(s => s.Criterion)
            .Where(s => s.Team.EventId == eventId)
            .ToListAsync(ct);

    public async Task<IEnumerable<Score>> GetByJudgeAndEventAsync(Guid judgeId, Guid eventId, CancellationToken ct = default)
        => await _db.Scores
            .Include(s => s.Judge)
            .Include(s => s.Team)
            .Include(s => s.Criterion)
            .Where(s => s.JudgeId == judgeId && s.Team.EventId == eventId)
            .ToListAsync(ct);

    public async Task<Score?> GetExistingAsync(Guid judgeId, Guid teamId, Guid criterionId, CancellationToken ct = default)
        => await _db.Scores
            .FirstOrDefaultAsync(s =>
                s.JudgeId == judgeId &&
                s.TeamId == teamId &&
                s.CriterionId == criterionId, ct);
}
