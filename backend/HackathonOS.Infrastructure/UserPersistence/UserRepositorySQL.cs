using Dapper;
using HackathonOS.Domain;
using HackathonOS.Domain.Entities;

namespace HackathonOS.Infrastructure.UserPersistence;

public class UserRepositorySql(ISharedDatabaseUtils utils) : IUserRepository
{
    public async Task<User?> CreateUserAsync(User request, CancellationToken ct = default)
    {
        using var connection = utils.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("first_name", request.FirstName);
        parameters.Add("last_name", request.LastName);
        parameters.Add("email", request.Email);
        parameters.Add("password_hash", request.PasswordHash);
        parameters.Add("role", request.Role);
        parameters.Add("is_active", request.IsActive);
        parameters.Add("guid", request.Guid);
        parameters.Add("created_on_utc", request.CreatedOnUtc);

        var sql = """
                      INSERT INTO users (
                          FIRST_NAME,
                          LAST_NAME,
                          EMAIL,
                          PASSWORD_HASH,
                          ROLE,
                          IS_ACTIVE,
                          GUID,
                          CREATED_ON_UTC
                      ) VALUES (
                          :first_name,
                          :last_name,
                          :email,
                          :password_hash,
                          :role,
                          :is_active,
                          :guid,
                          :inserted_on_utc
                      )
                  """;

        utils.ConfigureForDatabase(ref sql);
        var insertedId = await connection.ExecuteScalarAsync<int>(sql, parameters);
        request.Id = insertedId;
        return insertedId > 0 ? request : null;
    }

    public Task<Paginated<User>> GetAllUsersAsync(int pageNumber = 0, int pageSize = 100, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserDetailsAsync(Guid id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserDetailsAsync(string email, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<User?> UpdateUserAsync(Guid guid, User request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUserAsync(Guid guid, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}