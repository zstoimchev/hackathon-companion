using HackathonOS.Application.DTOs.Criteria;
using HackathonOS.Application.Interfaces;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Services;

public class CriterionService(
    IRepository<Criterion> criteria, IEventRepository events)
{
    public async Task<IEnumerable<CriterionResponse>> GetByEventAsync(Guid eventId, CancellationToken ct = default)
    {
        var evt = await events.GetWithDetailsAsync(eventId, ct)
            ?? throw new KeyNotFoundException($"Event {eventId} not found.");
        return evt.Criteria.Select(MapToResponse);
    }

    public async Task<CriterionResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var criterion = await criteria.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Criterion {id} not found.");
        return MapToResponse(criterion);
    }

    public async Task<CriterionResponse> CreateAsync(CreateCriterionRequest request, CancellationToken ct = default)
    {
        var evt = await events.GetByIdAsync(request.EventId, ct)
            ?? throw new KeyNotFoundException($"Event {request.EventId} not found.");

        var criterion = new Criterion
        {
            EventId = request.EventId,
            Name = request.Name,
            Description = request.Description,
            Weight = request.Weight
        };
        await criteria.AddAsync(criterion, ct);
        await criteria.SaveChangesAsync(ct);
        return MapToResponse(criterion);
    }

    public async Task<CriterionResponse> UpdateAsync(Guid id, UpdateCriterionRequest request, CancellationToken ct = default)
    {
        var criterion = await criteria.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Criterion {id} not found.");

        criterion.Name = request.Name;
        criterion.Description = request.Description;
        criterion.Weight = request.Weight;
        criterion.UpdatedAt = DateTime.UtcNow;

        criteria.Update(criterion);
        await criteria.SaveChangesAsync(ct);
        return MapToResponse(criterion);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var criterion = await criteria.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Criterion {id} not found.");
        criteria.Remove(criterion);
        await criteria.SaveChangesAsync(ct);
    }

    private static CriterionResponse MapToResponse(Criterion c) => new(
        c.Id, c.EventId, c.Name, c.Description, c.Weight, c.CreatedAt);
}
