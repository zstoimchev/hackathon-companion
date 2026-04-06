namespace HackathonOS.Domain.Configurations;

public class DatabaseOptions
{
    public DatabaseProvider Provider { get; set; }
    public ConnectionStringsOptions? ConnectionStrings { get; set; }
    public string DefaultPassword { get; set; } = string.Empty;
}

public class ConnectionStringsOptions
{
    public string SqLite { get; set; } = string.Empty;
    public string Postgres { get; set; } = string.Empty;
    public string Oracle { get; set; } = string.Empty;
}

public enum DatabaseProvider
{
    SqLite,
    Postgres,
    Oracle
}