using HackathonOS.Domain.Enums;

namespace HackathonOS.Domain.Entities;

public class Userr : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRolee Rolee { get; set; }
    public bool IsActive { get; set; } = true;
    
    // todo: need a team?
}