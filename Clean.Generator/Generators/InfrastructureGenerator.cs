using Clean.Generator.Helpers;
using Clean.Generator.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.SqlServer.Dac.Model;

namespace Clean.Generator.Generators
{
    public class InfrastructureGenerator
    {
        private readonly HashSet<string> _RestrictedNames = new()
        {
            "Attribute"
        };
        private readonly string _TemplateDirectory = $"{AppContext.BaseDirectory.Split("\\bin")[0]}\\Templates\\Infrastructure";
        private readonly string _DefaultNamespace;
        private readonly string _OutputDirectory;
        private readonly bool _IncludeSchemaLabel;

        private Context Context;
        private string _SaveLocation;

        public InfrastructureGenerator(IConfiguration config, Context context)
        {
            _IncludeSchemaLabel = bool.TryParse(config["Values:IncludeSchemaLabel"], out bool includeSchemaLabel) ? includeSchemaLabel : false;
            _DefaultNamespace = config["Values:DefaultNamespace"];
            _OutputDirectory = config["Values:InfrastructureOutputDirectory"];

            Context = context ?? throw new ArgumentNullException(nameof(context));
            _SaveLocation = $"{_OutputDirectory}\\{Context.Name}Context";

            HashSet<string> infrastructureTypes = config["Values:InfrastructureTypes"].Split(",").ToHashSet();

            if (infrastructureTypes.Contains(InfrastructureTypes.SQL.ToString()))
            {
                foreach(Models.Table table in Context.Tables)
                    GenerateTableConfiguration(table);

                GenerateDbContext();
                GenerateExtension();
            }
        }

        private void GenerateTableConfiguration(Models.Table table)
        {
            string templateText = FileEditor.ReadTemplateText(_TemplateDirectory, "\\Configurations\\TableNameConfiguration.cs");
            string tableName = GeneratorExtensions.GetTableName(_IncludeSchemaLabel, table);
            templateText = templateText.Replace("Clean.", _DefaultNamespace);
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("//RefTableName", (_RestrictedNames.Contains(tableName)
                ? $"using {tableName} = {_DefaultNamespace}Domain.{Context.Name}Context.Entities.{tableName};"
                : ""));
            templateText = templateText.Replace("TableName", tableName);

            StringBuilder keylessTables = new();
            StringBuilder tableKeys = new();
            StringBuilder foreignKeys = new();
            StringBuilder columns = new();
            if (table.IsKeyless)
                keylessTables.AppendLine($"\t\t\tbuilder.HasNoKey();");
            else
            {
                List<string> keys = table.Columns.Where(column => column.IsPrimaryKey).Select(column => GeneratorExtensions.GetPropertyNameFromColumnName(table, column.Name)).ToList();
                StringBuilder keyString = new();
                keyString.Append($"col.{keys[0]}");
                foreach (string key in keys.Skip(1))
                    keyString.Append($", col.{key}");

                tableKeys.AppendLine($"\t\t\tbuilder.HasKey(col => new {{ {keyString.ToString()} }});");
            }

            foreach (Models.Column column in table.Columns)
            {
                string columnName = GeneratorExtensions.GetPropertyNameFromColumnName(table, column.Name);
                columns.AppendLine($"\t\t\tbuilder" +
                    $".Property(c => c.{columnName})" +
                    (column.Name != columnName ? $".HasColumnName(\"{column.Name}\")" : "") +
                    (column.IsNullable ? "" : ".IsRequired()") +
                    (column.Length.HasValue ? $".HasMaxLength({column.Length})" : "") +
                    (column.Precision.HasValue ? $".HasPrecision({column.Precision}, {column.Scale ?? 0})" : "") +
                    (column.IsIdentity ? $".UseIdentityColumn({column.IdentitySeed ?? 1}, {column.IdentityIncrement ?? 1})" : "") +
                    (!string.IsNullOrEmpty(column.DefualtValue) ? $".HasDefaultValue({SQLConvertorHelper.ConvertDefaultToDataType(column.DataType, column.DefualtValue)})" : "") +
                    ";");
            }


            //ToDo: Remove hardcoded Many:1 Relationship definitions and check for proper types.
            foreach (ForeignKey foreignKey in table.ForeignKeys)
            {
                List<string> foreignColumns = foreignKey.ForeignColumns.Select(column => GeneratorExtensions.GetPropertyNameFromColumnName(table, column)).ToList();
                StringBuilder principalkeyString = new();
                principalkeyString.Append($"d.{foreignColumns[0]}");
                foreach (string key in foreignColumns.Skip(1))
                    principalkeyString.Append($", d.{key}");

                List<string> principalColumns = foreignKey.DefiningColumns.Select(column => GeneratorExtensions.GetPropertyNameFromColumnName(table, column)).ToList();
                StringBuilder foreignkeyString = new();
                foreignkeyString.Append($"p.{principalColumns[0]}");
                foreach (string key in principalColumns.Skip(1))
                    foreignkeyString.Append($", p.{key}");

                string foreignTableName = GeneratorExtensions.GetTableName(_IncludeSchemaLabel, foreignKey);
                foreignKeys.AppendLine($"\t\t\tbuilder"
                    + $".HasOne(d => d.{(tableName == foreignTableName ? "Parent" : "") + foreignTableName})"
                    + ".WithMany("
                    + (table.IsKeyless ? "" : $"p => p.{(tableName == foreignTableName ? "Related" : "") + tableName}") + ")"
                    + $".HasForeignKey(p => "
                    + (foreignColumns.Count == 1
                            ? foreignkeyString
                            : $"new {{ {foreignkeyString} }}")
                    + ")"
                    + (table.IsKeyless
                        ? ""
                        : $".HasPrincipalKey(d => " + (principalColumns.Count == 1
                            ? principalkeyString
                            : $"new {{ {principalkeyString} }}")
                    + ")")
                    + (foreignKey.Delete != ForeignKeyAction.NoAction
                        ? $".OnDelete({SQLConvertorHelper.GetDeleteBehavior(foreignKey.Delete)})"
                        : "")
                    + ";"
                    );
            }
            templateText = templateText.Replace("//TableKeys", tableKeys.ToString());
            templateText = templateText.Replace("//ForeignKeys", foreignKeys.ToString());
            templateText = templateText.Replace("//KeylessTables", keylessTables.ToString());
            templateText = templateText.Replace("//Columns", columns.ToString());

            FileEditor.WriteFile($"{_SaveLocation}\\Configurations", $"{tableName}Configuration.cs", templateText);
        }

        private void GenerateDbContext()
        {
            string templateText = FileEditor.ReadTemplateText(_TemplateDirectory, "ContextNameDbContext.cs");
            templateText = templateText.Replace("Clean.", _DefaultNamespace);
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("//ApplyConfigurations", (Context.Tables.Any()
                ? "\t\t\tmodelBuilder.ApplyConfigurationsFromAssembly(" + $"typeof({GeneratorExtensions.GetTableName(_IncludeSchemaLabel, Context.Tables.First())}" +
                    $"Configuration).Assembly);"
                : ""));

            StringBuilder refTables = new();
            StringBuilder tables = new();
            foreach (Models.Table table in Context.Tables)
            {
                string tableName = GeneratorExtensions.GetTableName(_IncludeSchemaLabel, table);

                if (_RestrictedNames.Contains(tableName))
                    refTables.AppendLine($"using {tableName} = {_DefaultNamespace}Domain.{Context.Name}Context.Entities.{tableName};");

                tables.AppendLine($"\t\tpublic DbSet<{tableName}>? {tableName} {{ get; set; }}");

            }
            templateText = templateText.Replace("//Tables", tables.ToString());
            templateText = templateText.Replace("//RefTableName", refTables.ToString());

            FileEditor.WriteFile($"{_SaveLocation}", $"{Context.Name}DbContext.cs", templateText);
        }

        private void GenerateExtension()
        {
            string templateText = FileEditor.ReadTemplateText(_TemplateDirectory, "ContextNameInfrastructureExtension.cs");
            templateText = templateText.Replace("Clean.", _DefaultNamespace);
            templateText = templateText.Replace("ContextName", Context.Name);

            StringBuilder refTables = new();
            StringBuilder scopedRepositories = new();
            foreach (Models.Table table in Context.Tables)
            {
                string tableName = GeneratorExtensions.GetTableName(_IncludeSchemaLabel, table);

                if (_RestrictedNames.Contains(tableName))
                    refTables.AppendLine($"using {tableName} = {_DefaultNamespace}Domain.{Context.Name}Context.Entities.{tableName};");

                scopedRepositories.AppendLine($"\t\t\tservices.AddScoped<IRepository<{tableName}>, SQLRepository<{Context.Name}DbContext, {tableName}>>();");
            }
            templateText = templateText.Replace("//ScopedRepositories", scopedRepositories.ToString());
            templateText = templateText.Replace("//RefTableName", refTables.ToString());

            FileEditor.WriteFile($"{_SaveLocation}", $"{Context.Name}InfrastructureExtension.cs", templateText);
        }
    }
}
