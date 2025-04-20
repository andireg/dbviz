using DatabaseVisualizer.DataAccess;
using DatabaseVisualizer.Domain;
using System.Data;

namespace DatabaseVisualizer;

internal sealed class DatabaseParser
    : IDatabaseParser, IDisposable
{
    private readonly IDbConnection dbConnection;

    public DatabaseParser(IDbFactory dbFactory)
    {
        dbConnection = dbFactory.CreateConnection();
        dbConnection.Open();
    }

    public IEnumerable<string>? ExcludeSchemas { get; set; }

    public IEnumerable<string>? IncludeSchemas { get; set; }

    public IEnumerable<string>? ExcludeColumns { get; set; }

    public IReadOnlyCollection<IDbItemInfo> GetItems() => [.. GetTables(), .. GetForeignKeys()];

    private List<IDbItemInfo> GetTables()
    {
        List<IDbItemInfo> items = [];
        IDbCommand tableCommand = dbConnection.CreateCommand();
        tableCommand.CommandText = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";
        using (IDataReader tableReader = tableCommand.ExecuteReader())
        {
            while (tableReader.Read())
            {
                string schema = tableReader.GetString(0);

                if (CheckFilters(schema))
                {
                    items.Add(new DbTableInfo(new DbTableName(schema, tableReader.GetString(1))));
                }
            }
        }

        IDbCommand columnCommand = dbConnection.CreateCommand();
        columnCommand.CommandText = """
SELECT COLUMNS.TABLE_SCHEMA, COLUMNS.TABLE_NAME, COLUMNS.COLUMN_NAME, IS_NULLABLE, COLUMN_DEFAULT, DATA_TYPE, KEY_COLUMN_USAGE.CONSTRAINT_NAME
  FROM INFORMATION_SCHEMA.COLUMNS 
  LEFT OUTER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ON COLUMNS.TABLE_SCHEMA = KEY_COLUMN_USAGE.TABLE_SCHEMA AND COLUMNS.TABLE_NAME = KEY_COLUMN_USAGE.TABLE_NAME AND COLUMNS.COLUMN_NAME = KEY_COLUMN_USAGE.COLUMN_NAME
 ORDER BY COLUMNS.TABLE_SCHEMA, COLUMNS.TABLE_NAME, COLUMNS.ORDINAL_POSITION
""";
        using (IDataReader columnReader = columnCommand.ExecuteReader())
        {
            while (columnReader.Read())
            {
                string tableSchema = columnReader.GetString(0);
                string tableName = columnReader.GetString(1);
                string columnName = columnReader.GetString(2);

                if (!CheckColumnFilter(columnName))
                {
                    continue;
                }

                if (items.FirstOrDefault(x =>
                    x is DbTableInfo table &&
                    table.Table.Schema == tableSchema && table.Table.Name == tableName) is not DbTableInfo dbTableInfo)
                {
                    continue;
                }

                List<DbColumnInfo> columns =
                    dbTableInfo.Columns as List<DbColumnInfo> ??
                    [];

                columns.Add(new DbColumnInfo(columnName, columnReader.GetString(5))
                {
                    IsNullable = columnReader.GetString(3) == "YES",
                    DefaultValue = columnReader.IsDBNull(4) ? null : columnReader.GetString(4),
                    IsPrimaryKey = !columnReader.IsDBNull(6) && columnReader.GetString(6)?.StartsWith("PK_") == true,
                });

                dbTableInfo.Columns = columns;
            }
        }

        return items;
    }

    private List<IDbItemInfo> GetForeignKeys()
    {
        List<DbForeignKeyInfo> items = [];
        IDbCommand tableCommand = dbConnection.CreateCommand();
        tableCommand.CommandText = """
SELECT RC.CONSTRAINT_NAME FK_Name
, KF.TABLE_SCHEMA FK_Schema
, KF.TABLE_NAME FK_Table
, KP.TABLE_SCHEMA PK_Schema
, KP.TABLE_NAME PK_Table
, KF.COLUMN_NAME FK_Column
, KP.COLUMN_NAME PK_Column
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KF ON RC.CONSTRAINT_NAME = KF.CONSTRAINT_NAME
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KP ON RC.UNIQUE_CONSTRAINT_NAME = KP.CONSTRAINT_NAME
""";
        using (IDataReader tableReader = tableCommand.ExecuteReader())
        {
            while (tableReader.Read())
            {
                string schema = tableReader.GetString(1);
                string schemaReferencedTableName = tableReader.GetString(3);
                if (CheckFilters(schema) && CheckFilters(schemaReferencedTableName))
                {
                    items.Add(new DbForeignKeyInfo(
                        tableReader.GetString(0),
                        new DbTableName(schema, tableReader.GetString(2)),
                        new DbTableName(schemaReferencedTableName, tableReader.GetString(4)))
                    {
                        Columns = [tableReader.GetString(5)],
                        ReferencedTableNameColumns = [tableReader.GetString(6)],
                    });
                }
            }
        }

        return items
            .GroupBy(item => item.Name)
            .Select(grp =>
            {
                DbForeignKeyInfo first = grp.First();
                first.Columns = grp.SelectMany(x => x.Columns).ToList();
                first.ReferencedTableNameColumns = grp.SelectMany(x => x.ReferencedTableNameColumns).ToList();
                return (IDbItemInfo)first;
            })
            .ToList();
    }

    private bool CheckColumnFilter(string columnName) =>
        ExcludeColumns?.All(s => s != columnName) != false;

    private bool CheckFilters(string schema) =>
        IncludeSchemas?.Any(s => s == schema) != false &&
        ExcludeSchemas?.All(s => s != schema) != false;

    public void Dispose()
    {
        dbConnection.Dispose();
    }
}
