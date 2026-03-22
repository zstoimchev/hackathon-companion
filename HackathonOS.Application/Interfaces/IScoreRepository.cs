using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Application.Interfaces;

public interface IScoreRepository : IRepository<Score>
{
    Task<IEnumerable<Score>> GetByEventAsync(Guid eventId, CancellationToken ct = default);
    Task<IEnumerable<Score>> GetByJudgeAndEventAsync(Guid judgeId, Guid eventId, CancellationToken ct = default);
    Task<Score?> GetExistingAsync(Guid judgeId, Guid teamId, Guid criterionId, CancellationToken ct = default);
}
