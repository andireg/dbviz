using DatabaseVisualizer.Domain;

namespace DatabaseVisualizer.Renderer;

public interface IRenderer
{
    Task RenderToFileAsync(string filePath, IEnumerable<IDbItemInfo> items, CancellationToken cancellationToken);

    void RenderToFile(string filePath, IEnumerable<IDbItemInfo> items);

    string Render(IEnumerable<IDbItemInfo> items);
}