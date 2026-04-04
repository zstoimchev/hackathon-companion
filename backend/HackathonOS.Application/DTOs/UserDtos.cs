using HackathonOS.Domain.Enums;

namespace HackathonOS.Application.DTOs;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    UserRole Role);

public record UserResponse(
    Guid Guid,
    string FirstName,
    string LastName,
    string Email,
    string Role
);