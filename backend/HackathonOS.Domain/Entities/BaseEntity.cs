namespace HackathonOS.Domain.Entities;

public class BaseEntity
{
    public int Id { get; set; }
    public Guid Guid { get; init; } = Guid.NewGuid();
    public DateTime CreatedOnUtc { get; } = DateTime.UtcNow;
    public DateTime? UpdatedOnUtc { get; set; }
    public DateTime? DeletedOnUtc { get; set; }
    public string? DeletedBy { get; set; }
}