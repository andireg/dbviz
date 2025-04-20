using System.Data;

namespace DatabaseVisualizer.DataAccess;

public interface IDbFactory
{
    IDbConnection CreateConnection();
}