namespace DatabaseVisualizer.Domain;

public interface IDbItemInfo
{
}

public record DbTableName(string Schema, string Name);

public class DbTableInfo(DbTableName table) : IDbItemInfo
{
    public DbTableName Table { get; set; } = table;

    public IReadOnlyCollection<DbColumnInfo> Columns { get; set; } = new List<DbColumnInfo>();
}

public class DbColumnInfo(string name, string type)
{
    public string Name { get; set; } = name;

    public string Type { get; set; } = type;

    public bool IsPrimaryKey { get; set; }

    public bool IsNullable { get; set; }

    public string? DefaultValue { get; set; }
}

public class DbForeignKeyInfo(string name, DbTableName table, DbTableName referencedTable) : IDbItemInfo
{
    public string Name { get; set; } = name;

    public DbTableName Table { get; set; } = table;

    public DbTableName ReferencedTable { get; set; } = referencedTable;

    public IReadOnlyCollection<string> Columns { get; set; } = new List<string>();

    public IReadOnlyCollection<string> ReferencedTableNameColumns { get; set; } = new List<string>();
}