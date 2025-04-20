using DatabaseVisualizer.Domain;
using DatabaseVisualizer.Renderer;
using DatabaseVisualizer.Renderer.Factory;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

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
        SqlConnection sqlConnection = new(appSettings.ConnectionString);
        await sqlConnection.OpenAsync(cancellationToken);
        try
        {
            databaseParser.IncludeSchemas = appSettings.IncludeSchemas.Split(';');
            databaseParser.ExcludeSchemas = appSettings.ExcludeSchemas.Split(';');
            databaseParser.ExcludeColumns = appSettings.ExcludeColumns.Split(';');
            dbItemInfo = databaseParser.GetItems();
        }
        finally
        {
            await sqlConnection.CloseAsync();
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