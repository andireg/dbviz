
using DatabaseVisualizer.Renderer.Metadata;
using System.Reflection;

namespace DatabaseVisualizer.Renderer.DependencyInjection;

public class RendererRegistration
{
    public RendererRegistration(Type type)
    {
        RenderNameAttribute? renderNameAttribute = type.GetCustomAttribute<RenderNameAttribute>();
        Name = renderNameAttribute?.Name ?? type.Name;
        ShortName = renderNameAttribute?.ShortName;
        RendererType = type;
    }

    public string Name { get; }

    public string? ShortName { get; }

    public Type RendererType { get; }
}