using System.Net.Http.Json;
using System.Text.Json;
using HackathonOS.Domain;
using HackathonOS.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HackathonOS.Infrastructure.UserPersistence;

public class UserRepositoryClient(
    ILoggerFactory loggerFactory,
    IHttpClientFactory httpClientFactory) : IUserRepository
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<UserRepositoryClient>();
    private readonly HttpClient _client = httpClientFactory.CreateClient("HackathonOS.DatabaseAPI");
    private const string RequestUri = "api/users";

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<User?> CreateUserAsync(
        User request,
        CancellationToken ct = default)
    {
        var response = await _client.PostAsJsonAsync(RequestUri, request, ct);
        var rawUser = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<User>(rawUser, _jsonOptions);
    }

    public async Task<Paginated<User>> GetAllUsersAsync(
        int pageNumber = 0,
        int pageSize = 100,
        CancellationToken ct = default)
    {
        var response = await _client.GetAsync($"{RequestUri}?pageNumber={pageNumber}&pageSize={pageSize}", ct);
        response.EnsureSuccessStatusCode();
        var rawUser = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<Paginated<User>>(rawUser, _jsonOptions)!;
    }

    public async Task<User?> GetUserDetailsAsync(
        Guid id,
        CancellationToken ct = default)
    {
        var response = await _client.GetAsync($"{RequestUri}/{id}", ct);
        var rawUser = await response.Content.ReadAsStringAsync(ct);
        return response.IsSuccessStatusCode
            ? JsonSerializer.Deserialize<User>(rawUser, _jsonOptions)
            : null;
    }

    public async Task<User?> GetUserDetailsAsync(
        string email,
        CancellationToken ct = default)
    {
        var response = await _client.GetAsync($"{RequestUri}/email/{email}", ct);
        var rawUser = await response.Content.ReadAsStringAsync(ct);
        return response.IsSuccessStatusCode
            ? JsonSerializer.Deserialize<User>(rawUser, _jsonOptions)
            : null;
    }

    public async Task<User?> UpdateUserAsync(
        Guid guid,
        User request,
        CancellationToken ct = default)
    {
        var response = await _client.PutAsJsonAsync($"{RequestUri}/{guid}", request, ct);
        var rawUser = await response.Content.ReadAsStringAsync(ct);
        return response.IsSuccessStatusCode
            ? JsonSerializer.Deserialize<User>(rawUser, _jsonOptions)
            : null;
    }

    public async Task<bool> DeleteUserAsync(
        Guid guid,
        CancellationToken ct = default)
    {
        var response = await _client.DeleteAsync($"{RequestUri}/{guid}", ct);
        return response.IsSuccessStatusCode;
    }
}