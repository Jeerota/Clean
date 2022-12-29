using Clean.Generator;
using Clean.Generator.Models;
using Microsoft.SqlServer.Dac.Model;
using System.Data;

HashSet<string> baseColumns = new()
{
    "CreatedDate",
    "ModifiedDate"
};

TSqlModel model = new(@"C:\Users\Justin\Source\Repos\Clean.Infrastructure\Dapacs\Clean.ExampleDb.dacpac");

Context context = new("Example");
IEnumerable<TSqlObject> sqlTables = model.GetObjects(DacQueryScopes.All, ModelSchema.Table);
foreach (TSqlObject sqlTable in sqlTables)
{
    Clean.Generator.Models.Table table = new(sqlTable.Name.Parts[1], sqlTable.Name.Parts[0]);

    IEnumerable<TSqlObject> sqlColumns = sqlTable.GetChildren().Where(child => child.ObjectType.Name == "Column");
    foreach (TSqlObject sqlColumn in sqlColumns)
    {
        string columnName = sqlColumn.Name.Parts[2];
        if (!baseColumns.Contains(columnName))
        {
            SqlDataType sqlDataType = (SqlDataType)sqlColumn
                .GetReferenced(Microsoft.SqlServer.Dac.Model.Column.DataType)
                .FirstOrDefault()?
                .GetProperty(DataType.SqlDataType);

            Clean.Generator.Models.Column column = new(columnName)
            {
                DataType = SQLConvertorHelper.GetCLRType(sqlDataType),
                Nullable = (bool)sqlColumn.GetProperty(Microsoft.SqlServer.Dac.Model.Column.Nullable),
                Identity = (bool)sqlColumn.GetProperty(Microsoft.SqlServer.Dac.Model.Column.IsIdentity)
            };

            table.Columns.Add(column);
        }
    }

    IEnumerable<TSqlObject> sqlForeignKeys = sqlTable.GetChildren().Where(child => child.ObjectType.Name == "ForeignKeyConstraint");
    foreach (TSqlObject sqlForeignKey in sqlForeignKeys)
    {
        ForeignKey foreignKey = new(sqlForeignKey.Name.Parts[1])
        {
            ForeignTable = sqlForeignKey.GetReferenced(Microsoft.SqlServer.Dac.Model.ForeignKeyConstraint.ForeignTable).FirstOrDefault()?.Name.Parts[1],
            DefiningTable = sqlTable.Name.Parts[1]
        };

        var foreignColumns = sqlForeignKey.GetReferenced(Microsoft.SqlServer.Dac.Model.ForeignKeyConstraint.ForeignColumns);
        var definingColumns = sqlForeignKey.GetReferenced(Microsoft.SqlServer.Dac.Model.ForeignKeyConstraint.Columns);

        foreignKey.ForeignColumns.AddRange(foreignColumns.Select(col => col.Name.Parts[2]));
        foreignKey.DefiningColumns.AddRange(definingColumns.Select(col => col.Name.Parts[2]));

        table.ForeignKeys.Add(foreignKey);
    }

    context.Tables.Add(table);
}

new DomainGenerator(context);
new InfrastructureGenerator(context);