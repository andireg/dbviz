namespace DatabaseVisualizer.Renderer.Factory;

public interface IRendererFactory
{
    IEnumerable<IRenderer> GetRenderers();
}