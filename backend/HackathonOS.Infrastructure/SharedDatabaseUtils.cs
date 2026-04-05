using System.Data;
using HackathonOS.Domain.Configurations;
using Microsoft.Extensions.Options;
using Npgsql;

namespace HackathonOS.Infrastructure;

public class SharedDatabaseUtils : ISharedDatabaseUtils
{
    private readonly DatabaseProvider _provider;
    private readonly string _connectionString;

    public SharedDatabaseUtils(IOptions<DatabaseOptions> options)
    {
        var db = options.Value;

        _provider = db.Provider;

        var template = _provider switch
        {
            DatabaseProvider.SqLite => db.ConnectionStrings?.SqLite,
            DatabaseProvider.Postgres => db.ConnectionStrings?.Postgres,
            DatabaseProvider.Oracle => db.ConnectionStrings?.Oracle,
            _ => throw new NotSupportedException($"Unsupported database: {_provider}")
        };

        if (string.IsNullOrWhiteSpace(template)) throw new InvalidOperationException("Missing ConnectionString!");

        var password = db.DefaultPassword;

        _connectionString = template.Contains("{0}")
            ? string.Format(template, password)
            : template;
    }

    public IDbConnection CreateConnection()
    {
        return _provider switch
        {
            // DatabaseProvider.SqLite => new SqliteConnection(_connectionString),
            DatabaseProvider.Postgres => new NpgsqlConnection(_connectionString),
            // DatabaseProvider.Oracle => new OracleConnection(_connectionString),
            _ => throw new NotSupportedException($"Unsupported database: {_provider}")
        };
    }

    public void ConfigureForDatabase(ref string sql)
    {
        sql += _provider switch
        {
            DatabaseProvider.Postgres => " RETURNING id;",
            _ => throw new NotSupportedException($"Unsupported database provider: {_provider}")
        };
    }
}