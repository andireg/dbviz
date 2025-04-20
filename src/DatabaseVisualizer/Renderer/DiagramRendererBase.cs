using DatabaseVisualizer.Domain;
using DatabaseVisualizer.Renderer.Metadata;
using System.Text;

namespace DatabaseVisualizer.Renderer;

public abstract class DiagramRendererBase : IRenderer
{
    [RenderParameter("ShowSchema", "schema")]
    public bool ShowSchema { get; set; } = true;

    public string Render(IEnumerable<IDbItemInfo> items)
    {
        StringBuilder stringBuilder = new();
        ProcessRendering(stringBuilder, items);
        return stringBuilder.ToString();
    }

    public void RenderToFile(string filePath, IEnumerable<IDbItemInfo> items)
    {
        StringBuilder stringBuilder = new();
        ProcessRendering(stringBuilder, items);
        File.WriteAllText(filePath, stringBuilder.ToString());
    }

    public async Task RenderToFileAsync(string filePath, IEnumerable<IDbItemInfo> items, CancellationToken cancellationToken)
    {
        StringBuilder stringBuilder = new();
        ProcessRendering(stringBuilder, items);
        await File.WriteAllTextAsync(filePath, stringBuilder.ToString(), cancellationToken);
    }

    protected abstract void ProcessRendering(StringBuilder stringBuilder, IEnumerable<IDbItemInfo> items);

    protected string GetTableName(DbTableName dbTableName)
        => ShowSchema ? $"{dbTableName.Schema}_{dbTableName.Name}" : dbTableName.Name;
}
