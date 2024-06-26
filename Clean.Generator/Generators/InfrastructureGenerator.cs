using Clean.Generator.Helpers;
using Clean.Generator.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Clean.Infrastructure.ContextNameContext.Entities;

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

        public InfrastructureGenerator(ref StringBuilder autoMapperConfig, IConfiguration config, Context context)
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
                {
                    GenerateEntity(table);
                    GenerateTableConfiguration(table);
                }

                GenerateAutoMapperConfiguration(ref autoMapperConfig);
                GenerateDbContext();
            }
        }
        private void GenerateEntity(Models.Table table)
        {
            if (Context == null)
                throw new ArgumentNullException(nameof(Context));

            string tableName = GeneratorExtensions.GetTableName(_IncludeSchemaLabel, table);
            string templateText = FileEditor.ReadTemplateText(_TemplateDirectory, "Entities\\TableName.cs");
            templateText = templateText.Replace("Clean.", _DefaultNamespace);
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("Table(\"TableName\"", $"Table(\"{table.Name}\"");
            templateText = templateText.Replace("//RefTableName", (_RestrictedNames.Contains(tableName)
                ? $"using {tableName} = {_DefaultNamespace}Domain.{Context.Name}Context.Entities.{tableName};"
                : ""));
            templateText = templateText.Replace("TableName", tableName);
            templateText = templateText.Replace("TableSchema", table.Schema);

            StringBuilder columnBuilder = new();
            StringBuilder columnCompareBuilder = new();
            StringBuilder columnValidateBuilder = new();
            foreach (Models.Column column in table.Columns)
            {
                if (!column.Validate())
                    throw new ArgumentException($"Invalid column: {column}");

                string columnName = GeneratorExtensions.GetPropertyNameFromColumnName(table, column.Name);

                //Columns
                if (columnName != column.Name)
                    columnBuilder.AppendLine($"\t\t[Column(\"{column.Name}\")]");

                columnBuilder.AppendLine($"\t\tpublic {column.DataType}? {columnName} {{ get; set; }}");

                //CompareColumns
                columnCompareBuilder.AppendLine();
                columnCompareBuilder.AppendLine($"\t\t\tif (other.{columnName} != null");
                columnCompareBuilder.AppendLine($"\t\t\t\t&& {columnName} != other.{columnName})");
                columnCompareBuilder.AppendLine($"\t\t\t{{");
                columnCompareBuilder.AppendLine($"\t\t\t\t{columnName} = other.{columnName};");
                columnCompareBuilder.AppendLine($"\t\t\t\tresult = true;");
                columnCompareBuilder.AppendLine($"\t\t\t}}");

                //ValidateColumns
                if (!column.IsNullable)
                {
                    if (column.IsPrimaryKey)
                        columnValidateBuilder.AppendLine($"\t\t\tif (isUpdate)");
                    else
                    {
                        if (column.DataType == "string")
                            columnValidateBuilder.AppendLine($"\t\t\tif (!isUpdate && string.IsNullOrWhiteSpace({columnName}))");
                        else if (column.DataType == "byte[]")
                            columnValidateBuilder.AppendLine($"\t\t\tif (!isUpdate && {columnName} == null)");
                        else
                            columnValidateBuilder.AppendLine($"\t\t\tif (!isUpdate && !{columnName}.HasValue)");
                    }
                    columnValidateBuilder.AppendLine($"\t\t\t\tresult.Errors.Add(\"{columnName} is required.\");");
                }
                if (column.Length.HasValue)
                {
                    columnValidateBuilder.AppendLine($"\t\t\tif ({columnName} != null && {columnName}.Length > {column.Length})");
                    columnValidateBuilder.AppendLine($"\t\t\t\tresult.Errors.Add(\"{columnName} exceeds maximum length of {column.Length}.\");");
                }
            }
            templateText = templateText.Replace("//Columns", columnBuilder.ToString());
            templateText = templateText.Replace("//CompareColumns", columnCompareBuilder.ToString());
            templateText = templateText.Replace("//ValidateColumns", columnValidateBuilder.ToString());

            StringBuilder foreignKeys = new();
            List<Models.ForeignKey> remoteForeignKeys = Context.Tables.SelectMany(tableRef => tableRef.ForeignKeys
                .Where(key => key.ForeignSchema == table.Schema && key.ForeignTable == table.Name)).ToList();
            HashSet<string> usedKeys = new(); foreach (Models.ForeignKey foreignKey in table.ForeignKeys)
            {
                if (!foreignKey.Validate())
                    throw new ArgumentException($"Invalid foreign key: {foreignKey}");

                string foreignTableName = GeneratorExtensions.GetTableName(_IncludeSchemaLabel, foreignKey);
                if (usedKeys.Add(foreignTableName)
                    && !Context.Tables.First(tableRef => tableRef.Schema == foreignKey.ForeignSchema && tableRef.Name == foreignKey.ForeignTable).IsKeyless)
                    foreignKeys.AppendLine($"\t\tpublic virtual {foreignTableName}? {(tableName == foreignTableName ? "Parent" : "") + foreignTableName} {{ get; set; }}");
            }
            usedKeys = new();
            foreach (Models.ForeignKey remoteForeignKey in remoteForeignKeys)
            {
                string foreignTableName = GeneratorExtensions.GetTableName(_IncludeSchemaLabel, remoteForeignKey, true);
                if (usedKeys.Add(foreignTableName)
                    && !Context.Tables.First(tableRef => tableRef.Schema == remoteForeignKey.DefiningSchema && tableRef.Name == remoteForeignKey.DefiningTable).IsKeyless)
                    foreignKeys.AppendLine($"\t\tpublic virtual ICollection<{foreignTableName}>? {(tableName == foreignTableName ? "Related" : "") + foreignTableName} {{ get; set; }}");
            }

            templateText = templateText.Replace("//ForeignKeys", foreignKeys.ToString());

            FileEditor.WriteFile($"{_SaveLocation}\\Entities", $"{tableName}.cs", templateText);
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

            templateText = templateText.Replace("//ToTable", $"\t\t\tbuilder.ToTable(\"{tableName}\", {table.Schema});");

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
            foreach (Models.ForeignKey foreignKey in table.ForeignKeys)
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

        private void GenerateAutoMapperConfiguration(ref StringBuilder autoMapperConfig)
        {
            foreach(Models.Table table in Context.Tables)
            {            
                string tableName = GeneratorExtensions.GetTableName(_IncludeSchemaLabel, table);
                autoMapperConfig.AppendLine($"\t\t\tcfg.CreateMap<{tableName}, {tableName}DTO>().ReverseMap();");
            }
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
    }
}
