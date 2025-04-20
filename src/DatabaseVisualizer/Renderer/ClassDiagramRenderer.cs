using DatabaseVisualizer.Domain;
using DatabaseVisualizer.Renderer.Metadata;
using System.Text;

namespace DatabaseVisualizer.Renderer;

[RenderName("ClassDiagram", "cls")]
public class ClassDiagramRenderer : DiagramRendererBase
{
    protected override void ProcessRendering(StringBuilder stringBuilder, IEnumerable<IDbItemInfo> items)
    {
        stringBuilder.AppendLine("classDiagram");
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

    private void RenderForeignKey(StringBuilder stringBuilder, DbForeignKeyInfo foreignKey, bool even)
    {
        if (even)
        {
            stringBuilder.AppendLine($"    {GetTableName(foreignKey.Table)} --|> {GetTableName(foreignKey.ReferencedTable)}");
        }
        else
        {
            stringBuilder.AppendLine($"    {GetTableName(foreignKey.ReferencedTable)} <|-- {GetTableName(foreignKey.Table)}");
        }
    }

    private void RenderTable(StringBuilder stringBuilder, DbTableInfo table)
    {
        stringBuilder.AppendLine($"    class {GetTableName(table.Table)} {{");
        if (table.Columns!= null)
        {
            foreach (DbColumnInfo column in table.Columns)
            {
                stringBuilder.AppendLine($"        {column.Type} {column.Name}");
            }
        }

        stringBuilder.AppendLine("    }");
    }
}
