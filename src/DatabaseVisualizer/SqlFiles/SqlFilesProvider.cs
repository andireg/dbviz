using Microsoft.Data.SqlClient;
using Squadron;
using System.Data;

namespace DatabaseVisualizer.SqlFiles;

public static class SqlFilesProvider
{
    private static readonly SqlServerResource sqlServerResource = new();

    public static async Task<IDbConnection> GetDatabaseConnectionAsync(IEnumerable<string> fileNames, CancellationToken cancellationToken)
    {
        string connectionString = await sqlServerResource.CreateDatabaseAsync();
        SqlConnection sqlConnection = new(connectionString);
        sqlConnection.Open();
        foreach (string file in fileNames)
        {
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = await File.ReadAllTextAsync(file, cancellationToken);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        return sqlConnection;
    }
}
