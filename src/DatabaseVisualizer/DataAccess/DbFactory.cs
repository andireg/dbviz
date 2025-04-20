using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace DatabaseVisualizer.DataAccess;

public class DbFactory(IOptions<AppSettings> appSettingsOptions) : IDbFactory
{
    public IDbConnection CreateConnection()
    {
        AppSettings appSettings = appSettingsOptions.Value;
        return new SqlConnection(appSettings.ConnectionString);
    }
}
