using DatabaseVisualizer.Domain;
using DatabaseVisualizer.Renderer.Metadata;
using System.Text;

namespace DatabaseVisualizer.Renderer;

[RenderName("erDiagram", "erd")]
public class ErDiagramRenderer : DiagramRendererBase
{
    [RenderParameter("ShowColumns", "cols")]
    public bool ShowColumns { get; set; } = true;

    [RenderParameter("ShowKeyName", "keys")]
    public bool ShowKeyName { get; set; } = true;

    protected override void ProcessRendering(StringBuilder stringBuilder, IEnumerable<IDbItemInfo> items)
    {
        stringBuilder.AppendLine("erDiagram");
        stringBuilder.AppendLine(string.Empty);

        int foreignKeyCounter = 0;
        foreach (IDbItemInfo item in items)
        {
            if (item is DbTableInfo table)
            {
                RenderTable(stringBuilder, table);
            }
            else if (item is DbForeignKeyInfo foreignKey)
            {
                RenderForeignKey(stringBuilder, foreignKey, foreignKeyCounter %2==0);
                foreignKeyCounter++;
            }
        }
    }

    protected void RenderForeignKey(StringBuilder stringBuilder, DbForeignKeyInfo foreignKey, bool even)
    {
        if (even)
        {
            stringBuilder.AppendLine($"    {GetTableName(foreignKey.Table)}  ||--o|  {GetTableName(foreignKey.ReferencedTable)} : \"{GetKeyName(foreignKey)}\"");
        }
        else
        {
            stringBuilder.AppendLine($"    {GetTableName(foreignKey.ReferencedTable)}  ||--o|  {GetTableName(foreignKey.Table)} : \"{GetKeyName(foreignKey)}\"");
        }
    }

    private string GetKeyName(DbForeignKeyInfo foreignKey)
        => ShowKeyName ? foreignKey.Name : string.Empty;

    protected void RenderTable(StringBuilder stringBuilder, DbTableInfo table)
    {
        stringBuilder.AppendLine($"    {GetTableName(table.Table)} {{");
        if (table.Columns!= null && ShowColumns)
        {
            foreach (DbColumnInfo column in table.Columns)
            {
                stringBuilder.AppendLine($"        {column.Type} {column.Name} {(column.IsPrimaryKey ? "PK" : string.Empty)}");
            }
        }

        stringBuilder.AppendLine("    }");
    }
}
