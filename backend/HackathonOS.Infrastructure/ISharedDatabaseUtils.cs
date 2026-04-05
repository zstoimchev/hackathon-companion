using System.Data;

namespace HackathonOS.Infrastructure;

public interface ISharedDatabaseUtils
{
    public IDbConnection CreateConnection();
}