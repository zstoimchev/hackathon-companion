using System;
using System.Collections.Generic;

namespace HackathonOS.Domain.Entities;

public class Criterion : BaseEntity
{
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    /// Relative weight 0.0–1.0. All criteria weights in an event should sum to 1.0.
    /// </summary>
    public double Weight { get; set; }

    // Navigation properties
    public ICollection<Score> Scores { get; set; } = new List<Score>();
}
