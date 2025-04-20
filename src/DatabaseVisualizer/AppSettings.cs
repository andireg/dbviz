namespace DatabaseVisualizer;

public class AppSettings
{
    public string ConnectionString { get; set; } = string.Empty;

    public string Cs { get => ConnectionString; set => ConnectionString = value; }

    public string IncludeSchemas { get; set; } = string.Empty;

    public string Is { get => IncludeSchemas; set => IncludeSchemas = value; }

    public string ExcludeSchemas { get; set; } = string.Empty;

    public string Es { get => ExcludeSchemas; set => ExcludeSchemas = value; }

    public string ExcludeColumns { get; set; } = string.Empty;

    public string Ec { get => ExcludeColumns; set => ExcludeColumns = value; }

    public string Render { get; set; } = string.Empty;

    public string R { get => Render; set => Render = value; }

    public string Output { get; set; } = string.Empty;

    public string O { get => Output; set => Output = value; }
}
