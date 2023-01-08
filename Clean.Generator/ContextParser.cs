using Clean.Generator.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Dac.Model;

namespace Clean.Generator
{
    public class ContextParser
    {
        private readonly HashSet<string> baseColumns = new()
            {
                "CreatedDate",
                "ModifiedDate"
            };

        private readonly string _DacPacDirectory;
        private readonly List<string> _DacPacPaths = new();

        public readonly List<Context> Contexts = new();

        public ContextParser(IConfiguration config)
        {
            _DacPacDirectory = config["Values:DacPacDirectory"];
            GetDacPacPaths();

            foreach(string path in _DacPacPaths)
                ParseContext(path);
        }

        private void GetDacPacPaths()
        {
            _DacPacPaths.AddRange(Directory.GetFiles(_DacPacDirectory, "*.dacpac"));
        }

        private void ParseContext(string dacPacPath)
        {
            TSqlModel model = new(dacPacPath);

            Context context = new(dacPacPath.Split(@"\").Last().Replace(".dacpac", ""));

            if (context.Name.Contains('.'))
                throw new ArgumentException("Context name cannot contain a '.', please rename the dacpac.");

            IEnumerable<TSqlObject> sqlTables = model.GetObjects(DacQueryScopes.All, ModelSchema.Table);
            foreach (TSqlObject sqlTable in sqlTables)
            {
                Models.Table table = new(sqlTable.Name.Parts[1], sqlTable.Name.Parts[0]);

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

                        Models.Column column = new(columnName)
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
                        ForeignTable = sqlForeignKey.GetReferenced(ForeignKeyConstraint.ForeignTable).FirstOrDefault()?.Name.Parts[1],
                        DefiningTable = sqlTable.Name.Parts[1]
                    };

                    var foreignColumns = sqlForeignKey.GetReferenced(ForeignKeyConstraint.ForeignColumns);
                    var definingColumns = sqlForeignKey.GetReferenced(ForeignKeyConstraint.Columns);

                    foreignKey.ForeignColumns.AddRange(foreignColumns.Select(col => col.Name.Parts[2]));
                    foreignKey.DefiningColumns.AddRange(definingColumns.Select(col => col.Name.Parts[2]));

                    table.ForeignKeys.Add(foreignKey);
                }

                context.Tables.Add(table);
            }

            Contexts.Add(context);
        }
    }
}
