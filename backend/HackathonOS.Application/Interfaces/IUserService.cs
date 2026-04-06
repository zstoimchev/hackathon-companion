using HackathonOS.Application.DTOs;
using HackathonOS.Domain;

namespace HackathonOS.Application.Interfaces;

public interface IUserService
{
    Task<GdgResult<UserResponse>> CreateUserAsync(
        UserRequest request,
        CancellationToken ct = default);

    Task<GdgResult<Paginated<UserResponse>>> GetAllUsersAsync(
        int pageNumber = 0,
        int pageSize = 100,
        CancellationToken ct = default);

    Task<GdgResult<UserResponse>> GetUserDetailsAsync(
        Guid guid,
        CancellationToken ct = default);

    Task<GdgResult<UserResponse>> UpdateUserAsync(
        Guid id,
        UserRequest request,
        CancellationToken ct = default);

    Task<GdgResult<UserResponse>> DeleteUserAsync(
        Guid id,
        CancellationToken ct = default);
}