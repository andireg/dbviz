namespace DatabaseVisualizer.Renderer.Metadata;

[AttributeUsage(AttributeTargets.Class)]
public class RenderNameAttribute(string name, string? shortName = null) : Attribute
{
    public string Name { get; } = name;

    public string? ShortName { get; } = shortName;
}
