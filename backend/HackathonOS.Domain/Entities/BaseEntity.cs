namespace HackathonOS.Domain.Entities;

public abstract class BaseEntity
{
    public long Id { get; set; }
    public Guid Guid { get; init; } = Guid.NewGuid();
    public DateTime CreatedOnUtc { get; } = DateTime.UtcNow;
    public DateTime UpdatedOnUtc { get; set; }
    public DateTime DeletedOnUtc { get; set; }
    public string DeletedBy { get; set; } = string.Empty;
}