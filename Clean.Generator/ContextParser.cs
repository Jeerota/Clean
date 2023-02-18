using Clean.Generator.Helpers;
using Clean.Generator.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Dac.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clean.Generator
{
    public class ContextParser
    {
        private readonly HashSet<string> baseColumns = new()
            {
            };

        private readonly string _DacPacDirectory;
        private readonly List<string> _DacPacPaths = new();
        private readonly HashSet<string> _Schemas;
        private readonly HashSet<string> _Tables;

        public readonly List<Context> Contexts = new();

        public ContextParser(IConfiguration config)
        {
            _DacPacDirectory = config["Values:DacPacDirectory"];
            _Schemas = config["Values:Schemas"]?.ToLowerInvariant()?.Split(",")?.ToHashSet() ?? new();
            _Tables = config["Values:Tables"]?.ToLowerInvariant()?.Split(",")?.ToHashSet() ?? new();
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
                if ((!_Schemas.Contains("*")
                        && !_Schemas.Contains(sqlTable.Name.Parts[0].ToLowerInvariant()))
                    || (!_Tables.Contains("*")
                        && !_Tables.Contains(sqlTable.Name.Parts[1].ToLowerInvariant())))
                    continue;

                Models.Table table = new(sqlTable.Name.Parts[1], sqlTable.Name.Parts[0].ToLowerInvariant());

                IEnumerable<TSqlObject> sqlPrimaryKeyColumns = sqlTable.GetChildren()
                    .Where(child => child.ObjectType.Name == "PrimaryKeyConstraint")
                    .SelectMany(keys => keys.GetReferenced(PrimaryKeyConstraint.Columns));
                IEnumerable<TSqlObject> sqlColumns = sqlTable.GetChildren().Where(child => child.ObjectType.Name == "Column");
                foreach (TSqlObject sqlColumn in sqlColumns)
                {
                    string columnName = sqlColumn.Name.Parts[2];
                    if (!baseColumns.Contains(columnName))
                    {
                        TSqlObject? columnDataType = sqlColumn
                            .GetReferenced(Microsoft.SqlServer.Dac.Model.Column.DataType)
                            .FirstOrDefault();

                        SqlDataType sqlDataType = columnDataType != null && columnDataType.ObjectType.Name == "DataType"
                            ? (SqlDataType)columnDataType.GetProperty(DataType.SqlDataType)
                            : SqlDataType.Unknown;

                        Models.Column column = new(columnName)
                        {
                            DataType = SQLConvertorHelper.GetCLRType(sqlDataType),
                            IsPrimaryKey = sqlPrimaryKeyColumns.Any(col => col.Name.Parts[2] == columnName),
                            IsNullable = (bool)sqlColumn.GetProperty(Microsoft.SqlServer.Dac.Model.Column.Nullable),
                            IsIdentity = (bool)sqlColumn.GetProperty(Microsoft.SqlServer.Dac.Model.Column.IsIdentity),
                            IdentityIncrement = (bool)sqlColumn.GetProperty(Microsoft.SqlServer.Dac.Model.Column.IsIdentity)
                                ? int.TryParse((string?)sqlColumn.GetProperty(Microsoft.SqlServer.Dac.Model.Column.IdentityIncrement), out int increment) 
                                    ? increment 
                                    : (int?)null
                                : (int?)null,
                            IdentitySeed = (bool)sqlColumn.GetProperty(Microsoft.SqlServer.Dac.Model.Column.IsIdentity)
                                ? int.TryParse((string?)sqlColumn.GetProperty(Microsoft.SqlServer.Dac.Model.Column.IdentitySeed), out int seed)
                                    ? seed
                                    : (int?)null
                                : (int?)null,
                            Length = (int)sqlColumn.GetProperty(Microsoft.SqlServer.Dac.Model.Column.Length) == 0 
                                ? (int?)null 
                                : (int)sqlColumn.GetProperty(Microsoft.SqlServer.Dac.Model.Column.Length),
                            Precision = (int)sqlColumn.GetProperty(Microsoft.SqlServer.Dac.Model.Column.Precision) == 0
                                ? (int?)null
                                : (int)sqlColumn.GetProperty(Microsoft.SqlServer.Dac.Model.Column.Precision),
                            Scale = (int)sqlColumn.GetProperty(Microsoft.SqlServer.Dac.Model.Column.Scale) == 0
                                ? (int?)null
                                : (int)sqlColumn.GetProperty(Microsoft.SqlServer.Dac.Model.Column.Scale),
                            DefualtValue = null //ToDo: Get Default Value.
                        };

                        table.Columns.Add(column);
                    }
                }

                IEnumerable<TSqlObject> sqlForeignKeys = sqlTable.GetChildren().Where(child => child.ObjectType.Name == "ForeignKeyConstraint");
                int idx = 0;
                foreach (TSqlObject sqlForeignKey in sqlForeignKeys)
                {
                    if ((!_Schemas.Contains("*")
                            && !_Schemas.Contains(sqlTable.Name.Parts[0].ToLowerInvariant()))
                        || (!_Tables.Contains("*")
                            && !_Tables.Contains(sqlForeignKey.GetReferenced(ForeignKeyConstraint.ForeignTable).FirstOrDefault()?.Name.Parts[1].ToLowerInvariant() ?? "")))
                        continue;

                    string sqlForeignKeyName = sqlForeignKey.Name.HasName 
                        ? sqlForeignKey.Name.Parts[1]
                        : $"Unnamed{idx}";
                    ForeignKey foreignKey = new(sqlForeignKeyName)
                    {
                        ForeignSchema = sqlForeignKey.GetReferenced(ForeignKeyConstraint.ForeignTable).FirstOrDefault()?.Name.Parts[0].ToLowerInvariant(),
                        ForeignTable = sqlForeignKey.GetReferenced(ForeignKeyConstraint.ForeignTable).FirstOrDefault()?.Name.Parts[1],
                        DefiningSchema = sqlTable.Name.Parts[0].ToLowerInvariant(),
                        DefiningTable = sqlTable.Name.Parts[1],
                        Delete = sqlForeignKey.GetProperty<ForeignKeyAction>(ForeignKeyConstraint.DeleteAction),
                        Update = sqlForeignKey.GetProperty<ForeignKeyAction>(ForeignKeyConstraint.UpdateAction)
                    };

                    var foreignColumns = sqlForeignKey.GetReferenced(ForeignKeyConstraint.ForeignColumns);
                    var definingColumns = sqlForeignKey.GetReferenced(ForeignKeyConstraint.Columns);

                    foreignKey.ForeignColumns.AddRange(foreignColumns.Select(col => col.Name.Parts[2]));
                    foreignKey.DefiningColumns.AddRange(definingColumns.Select(col => col.Name.Parts[2]));

                    table.ForeignKeys.Add(foreignKey);

                    if (!sqlForeignKey.Name.HasName)
                        idx++;
                }

                context.Tables.Add(table);
            }

            Contexts.Add(context);
        }
    }
}
