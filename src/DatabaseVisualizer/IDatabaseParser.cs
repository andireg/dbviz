using DatabaseVisualizer.Domain;

namespace DatabaseVisualizer;

internal interface IDatabaseParser
{
    IEnumerable<string>? IncludeSchemas { get; set; }

    IEnumerable<string>? ExcludeSchemas { get; set; }

    IEnumerable<string>? ExcludeColumns { get; set; }

    IReadOnlyCollection<IDbItemInfo> GetItems();
}