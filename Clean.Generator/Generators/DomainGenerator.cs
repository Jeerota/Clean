using Clean.Generator.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using Clean.Generator.Helpers;

namespace Clean.Generator.Generators
{
    public class DomainGenerator
    {
        private readonly HashSet<string> _RestrictedNames = new()
        {
            "Attribute"
        };
        private readonly string _TemplateDirectory = $"{AppContext.BaseDirectory.Split("\\bin")[0]}\\Templates\\Domain";
        private readonly string _DefaultNamespace;
        private readonly string? _OutputDirectory;
        private readonly bool _IncludeSchemaLabel;

        private readonly Context? Context;
        private readonly string? _SaveLocation;
        private readonly List<string>? _ScopedServices;

        public DomainGenerator(IConfiguration config, Context context)
        {
            _IncludeSchemaLabel = bool.TryParse(config["Values:IncludeSchemaLabel"], out bool includeSchemaLabel) ? includeSchemaLabel : false;
            _DefaultNamespace = config["Values:DefaultNamespace"];
            _OutputDirectory = config["Values:DomainOutputDirectory"];

            Context = context ?? throw new ArgumentNullException(nameof(context));
            _SaveLocation = $"{_OutputDirectory}\\{Context.Name}Context";

            HashSet<string> domainTypes = config["Values:DomainTypes"].Split(",").ToHashSet();

            if (domainTypes.Contains(DomainTypes.Standard.ToString()))
            {
                _ScopedServices = new();
                foreach (Table table in context.Tables)
                {
                    GenerateEntity(table);
                    GenerateLookupRequest(table);
                    GenerateService(table);
                }

                GenerateExtension();
            }
        }

        private void GenerateExtension()
        {
            if (Context == null)
                throw new ArgumentNullException(nameof(Context));
            if (_ScopedServices == null)
                throw new ArgumentNullException(nameof(_ScopedServices));

            string? templateText = FileEditor.ReadTemplateText(_TemplateDirectory, "Extensions\\ContextNameDomainExtension.cs");
            templateText = templateText.Replace("Clean.", _DefaultNamespace);
            templateText = templateText.Replace("ContextName", Context.Name);

            StringBuilder scopedServices = new();
            foreach (string service in _ScopedServices)
            {
                scopedServices.AppendLine(service);
            }
            templateText = templateText.Replace("//ScopedServices", scopedServices.ToString());

            FileEditor.WriteFile($"{_SaveLocation}\\Extensions", $"{Context.Name}DomainExtension.cs", templateText);
        }

        private void GenerateEntity(Table table)
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
            foreach (Column column in table.Columns)
            {
                if(!column.Validate())
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
                        else if(column.DataType == "byte[]")
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
            List<ForeignKey> remoteForeignKeys = Context.Tables.SelectMany(tableRef => tableRef.ForeignKeys
                .Where(key => key.ForeignSchema == table.Schema && key.ForeignTable == table.Name)).ToList();
            HashSet<string> usedKeys = new(); foreach (ForeignKey foreignKey in table.ForeignKeys)
            {
                if (!foreignKey.Validate())
                    throw new ArgumentException($"Invalid foreign key: {foreignKey}");

                string foreignTableName = GeneratorExtensions.GetTableName(_IncludeSchemaLabel, foreignKey);
                if (usedKeys.Add(foreignTableName)
                    && !Context.Tables.First(tableRef => tableRef.Schema == foreignKey.ForeignSchema && tableRef.Name == foreignKey.ForeignTable).IsKeyless)
                    foreignKeys.AppendLine($"\t\tpublic virtual {foreignTableName}? {(tableName == foreignTableName ? "Parent" : "") + foreignTableName} {{ get; set; }}");
            }
            usedKeys = new();
            foreach (ForeignKey remoteForeignKey in remoteForeignKeys)
            {
                string foreignTableName = GeneratorExtensions.GetTableName(_IncludeSchemaLabel, remoteForeignKey, true);
                if (usedKeys.Add(foreignTableName)
                    && !Context.Tables.First(tableRef => tableRef.Schema == remoteForeignKey.DefiningSchema && tableRef.Name == remoteForeignKey.DefiningTable).IsKeyless)
                    foreignKeys.AppendLine($"\t\tpublic virtual ICollection<{foreignTableName}>? {(tableName == foreignTableName ? "Related" : "") + foreignTableName} {{ get; set; }}");
            }

            templateText = templateText.Replace("//ForeignKeys", foreignKeys.ToString());

            FileEditor.WriteFile($"{_SaveLocation}\\Entities", $"{tableName}.cs", templateText);
        }

        private void GenerateLookupRequest(Table table)
        {
            string tableName = GeneratorExtensions.GetTableName(_IncludeSchemaLabel, table);
            string templateText = FileEditor.ReadTemplateText(_TemplateDirectory, "Models\\LookupRequests\\TableNameLookupRequest.cs");
            templateText = templateText.Replace("Clean.", _DefaultNamespace);
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("TableName", tableName);

            StringBuilder columnBuilder = new();
            foreach (Column column in table.Columns)
            {
                string columnName = GeneratorExtensions.GetPropertyNameFromColumnName(table, column.Name);

                columnBuilder.AppendLine($"\t\tpublic {column.DataType}? {columnName} {{ get; set; }}");
            }
            templateText = templateText.Replace("//Columns", columnBuilder.ToString());

            FileEditor.WriteFile($"{_SaveLocation}\\Models\\LookupRequests", $"{tableName}LookupRequest.cs", templateText);
        }

        private void GenerateService(Table table)
        {
            if (Context == null)
                throw new ArgumentNullException(nameof(Context));
            if (_ScopedServices == null)
                throw new ArgumentNullException(nameof(_ScopedServices));

            string tableName = GeneratorExtensions.GetTableName(_IncludeSchemaLabel, table);
            string templateText = FileEditor.ReadTemplateText(_TemplateDirectory, "Services\\TableNameService.cs");
            templateText = templateText.Replace("Clean.", _DefaultNamespace);
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("TableName", tableName);

            FileEditor.WriteFile($"{_SaveLocation}\\Services", $"{tableName}Service.cs", templateText);
            _ScopedServices.Add($"\t\t\tservices.AddScoped<I{tableName}Service, {tableName}Service>();");
        }
    }
}
