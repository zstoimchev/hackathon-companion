using System;

namespace HackathonOS.Domain.Entities;

public class Score : BaseEntity
{
    public Guid JudgeId { get; set; }
    public User Judge { get; set; } = null!;

    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public Guid CriterionId { get; set; }
    public Criterion Criterion { get; set; } = null!;

    /// <summary>
    /// Raw score, typically 1–10.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Optional comment from the judge for this score.
    /// </summary>
    public string? Comment { get; set; }
}
