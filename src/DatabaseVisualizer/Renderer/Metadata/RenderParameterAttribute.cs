namespace DatabaseVisualizer.Renderer.Metadata;

[AttributeUsage(AttributeTargets.Property)]
public class RenderParameterAttribute(string name, string? shortName = null) : Attribute
{
    public string Name { get; } = name;

    public string? ShortName { get; } = shortName;
}
