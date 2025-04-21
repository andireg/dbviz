using DatabaseVisualizer.Domain;
using DatabaseVisualizer.Renderer;
using DatabaseVisualizer.Renderer.Factory;
using DatabaseVisualizer.SqlFiles;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace DatabaseVisualizer;

internal class Launcher(
    IOptions<AppSettings> appSettingsOptions,
    IDatabaseParser databaseParser,
    IRendererFactory rendererFactory)
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        AppSettings appSettings = appSettingsOptions.Value;

        IReadOnlyCollection<IDbItemInfo> dbItemInfo;

        IDbConnection dbConnection;
        if (!string.IsNullOrWhiteSpace(appSettings.ConnectionString))
        {
            dbConnection = new SqlConnection(appSettings.ConnectionString);
            dbConnection.Open();
        }
        else if (!string.IsNullOrWhiteSpace(appSettings.SqlFiles))
        {
            dbConnection = await SqlFilesProvider.GetDatabaseConnectionAsync(appSettings.SqlFiles.Split(';'), cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("Not connectionString not sql files defined");
        }

        try
        {
            databaseParser.IncludeSchemas = appSettings.IncludeSchemas.Split(';');
            databaseParser.ExcludeSchemas = appSettings.ExcludeSchemas.Split(';');
            databaseParser.ExcludeColumns = appSettings.ExcludeColumns.Split(';');
            dbItemInfo = databaseParser.GetItems();
        }
        finally
        {
            dbConnection.Close();
        }

        string[] outputFiles = appSettings.Output.Split(';');

        int index = 0;
        IEnumerable<IRenderer> renderers = rendererFactory.GetRenderers();
        foreach (IRenderer renderer in renderers)
        {
            renderer.RenderToFile(outputFiles[index], dbItemInfo);
            index++;
        }
    }
}