using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Application.Interfaces;

public interface IMentorRequestRepository : IRepository<MentorRequest>
{
    Task<IEnumerable<MentorRequest>> GetByEventAsync(Guid eventId, CancellationToken ct = default);
    Task<IEnumerable<MentorRequest>> GetWaitingAsync(Guid eventId, CancellationToken ct = default);
    Task<IEnumerable<MentorRequest>> GetByMentorAsync(Guid mentorId, CancellationToken ct = default);
}
