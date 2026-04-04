using HackathonOS.Application.DTOs.Criteria;
using HackathonOS.Application.Interfaces;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Application.Services;

public class CriterionService
{
    private readonly IRepository<Criterion> _criteria;
    private readonly IEventRepository _events;

    public CriterionService(IRepository<Criterion> criteria, IEventRepository events)
    {
        _criteria = criteria;
        _events = events;
    }

    public async Task<IEnumerable<CriterionResponse>> GetByEventAsync(Guid eventId, CancellationToken ct = default)
    {
        var evt = await _events.GetWithDetailsAsync(eventId, ct)
            ?? throw new KeyNotFoundException($"Event {eventId} not found.");
        return evt.Criteria.Select(MapToResponse);
    }

    public async Task<CriterionResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var criterion = await _criteria.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Criterion {id} not found.");
        return MapToResponse(criterion);
    }

    public async Task<CriterionResponse> CreateAsync(CreateCriterionRequest request, CancellationToken ct = default)
    {
        var evt = await _events.GetByIdAsync(request.EventId, ct)
            ?? throw new KeyNotFoundException($"Event {request.EventId} not found.");

        var criterion = new Criterion
        {
            EventId = request.EventId,
            Name = request.Name,
            Description = request.Description,
            Weight = request.Weight
        };
        await _criteria.AddAsync(criterion, ct);
        await _criteria.SaveChangesAsync(ct);
        return MapToResponse(criterion);
    }

    public async Task<CriterionResponse> UpdateAsync(Guid id, UpdateCriterionRequest request, CancellationToken ct = default)
    {
        var criterion = await _criteria.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Criterion {id} not found.");

        criterion.Name = request.Name;
        criterion.Description = request.Description;
        criterion.Weight = request.Weight;
        criterion.UpdatedOnUtc = DateTime.UtcNow;

        _criteria.Update(criterion);
        await _criteria.SaveChangesAsync(ct);
        return MapToResponse(criterion);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var criterion = await _criteria.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Criterion {id} not found.");
        _criteria.Remove(criterion);
        await _criteria.SaveChangesAsync(ct);
    }

    private static CriterionResponse MapToResponse(Criterion c) => new(
        c.Guid, c.EventId, c.Name, c.Description, c.Weight, c.CreatedOnUtc);
}
