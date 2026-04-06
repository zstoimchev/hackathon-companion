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
                first_name,
                last_name,
                email,
                password_hash,
                role,
                is_active,
                guid,
                created_on_utc
            )
            VALUES (
                :first_name,
                :last_name,
                :email,
                :password_hash,
                :role,
                :is_active,
                :guid,
                :created_on_utc
            )
            RETURNING id;
        """;

        utils.ConfigureForDatabase(ref sql);

        var insertedId = await connection.ExecuteScalarAsync<int>(sql, parameters);
        request.Id = insertedId;

        return insertedId > 0 ? request : null;
    }

    public async Task<Paginated<User>> GetAllUsersAsync(
        int pageNumber = 0,
        int pageSize = 100,
        CancellationToken ct = default)
    {
        using var connection = utils.CreateConnection();

        var offset = pageNumber * pageSize;

        var sql = """
            SELECT *
            FROM users
            WHERE deleted_on_utc IS NULL
            ORDER BY created_on_utc DESC
            OFFSET :offset ROWS FETCH NEXT :page_size ROWS ONLY;

            SELECT COUNT(*)
            FROM users
            WHERE deleted_on_utc IS NULL;
        """;

        utils.ConfigureForDatabase(ref sql);

        await using var multi = await connection.QueryMultipleAsync(sql, new
        {
            offset,
            page_size = pageSize
        });

        var users = (await multi.ReadAsync<User>()).ToList();
        var total = await multi.ReadSingleAsync<int>();

        return new Paginated<User>
        {
            Items = users,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<User?> GetUserDetailsAsync(Guid guid, CancellationToken ct = default)
    {
        using var connection = utils.CreateConnection();

        var sql = """
            SELECT *
            FROM users
            WHERE guid = :guid
              AND deleted_on_utc IS NULL;
        """;

        utils.ConfigureForDatabase(ref sql);

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { guid });
    }

    public async Task<User?> GetUserDetailsAsync(string email, CancellationToken ct = default)
    {
        using var connection = utils.CreateConnection();

        var sql = """
            SELECT *
            FROM users
            WHERE email = :email
              AND deleted_on_utc IS NULL;
        """;

        utils.ConfigureForDatabase(ref sql);

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { email });
    }

    public async Task<User?> UpdateUserAsync(Guid guid, User request, CancellationToken ct = default)
    {
        using var connection = utils.CreateConnection();

        var sql = """
            UPDATE users
            SET
                first_name = :first_name,
                last_name = :last_name,
                email = :email,
                password_hash = :password_hash,
                role = :role,
                is_active = :is_active,
                updated_on_utc = :updated_on_utc
            WHERE guid = :guid
              AND deleted_on_utc IS NULL
            RETURNING *;
        """;

        utils.ConfigureForDatabase(ref sql);

        return await connection.QueryFirstOrDefaultAsync<User>(sql, new
        {
            guid,
            first_name = request.FirstName,
            last_name = request.LastName,
            email = request.Email,
            password_hash = request.PasswordHash,
            role = request.Role,
            is_active = request.IsActive,
            updated_on_utc = DateTime.UtcNow
        });
    }

    public async Task<bool> DeleteUserAsync(Guid guid, CancellationToken ct = default)
    {
        using var connection = utils.CreateConnection();

        var sql = """
            UPDATE users
            SET
                deleted_on_utc = :deleted_on_utc
            WHERE guid = :guid
              AND deleted_on_utc IS NULL;
        """;

        utils.ConfigureForDatabase(ref sql);

        var affected = await connection.ExecuteAsync(sql, new
        {
            guid,
            deleted_on_utc = DateTime.UtcNow
        });

        return affected > 0;
    }
}