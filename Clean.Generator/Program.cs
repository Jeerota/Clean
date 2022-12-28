using Clean.Generator;
using Clean.Generator.Models;
using Microsoft.SqlServer.Dac.Model;

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
            Clean.Generator.Models.Column column = new(columnName);

            foreach (var property in sqlColumn.ObjectType.Properties)
                column.Properties.Add(property.Name);

            foreach (var metaData in sqlColumn.ObjectType.Metadata)
                column.Metadata.Add(metaData.Name);

            table.Columns.Add(column);
        }

    }

    context.Tables.Add(table);
}

new DomainGenerator(context);
new InfrastructureGenerator(context);