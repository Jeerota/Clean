using Clean.Generator.Models;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Clean.Generator
{
    public class DomainGenerator
    {
        private const string _TemplateDirectory = "C:\\Users\\Justin\\Source\\Repos\\Clean.Infrastructure\\Clean.Generator\\Templates\\Domain";
        private readonly string _OutputDirectory;

        private Context Context;
        private string _SaveLocation;
        private List<string> _ScopedServices;
        private Dictionary<string, List<ForeignKey>> _TableForeignKeysMap;

        public DomainGenerator(IConfiguration config, Context context)
        {
            _OutputDirectory = config["Values:DomainOutputDirectory"];

            Context = context ?? throw new ArgumentNullException(nameof(context));
            _SaveLocation = $"{_OutputDirectory}\\{Context.Name}Context";

            _ScopedServices = new();
            _TableForeignKeysMap = new();
            foreach (Table table in context.Tables)
            {
                GenerateEntity(table);
                GenerateLookupRequest(table);
                GenerateService(table);
            }

            UpdateForeignForeignKeys();
            GenerateExtension();
        }

        private static string ReadTemplateText(string templateLocation)
        {
            StreamReader reader = new($"{_TemplateDirectory}\\{templateLocation}");
            string text = reader.ReadToEnd();
            reader.Close();
            return text;
        }
        private string ReadEntityText(string tableName)
        {
            StreamReader reader = new($"{_SaveLocation}\\Entities\\{tableName}.cs");
            string text = reader.ReadToEnd();
            reader.Close();
            return text;
        }


        private static void WriteFile(string fileLocation, string fileName, string fileContent)
        {
            if (!new FileInfo(fileLocation).Exists)
                Directory.CreateDirectory(fileLocation);

            StreamWriter writer = new($"{fileLocation}\\{fileName}");
            writer.Write(fileContent);
            writer.Flush();
            writer.Close();
        }

        private void UpdateForeignForeignKeys()
        {
            foreach(KeyValuePair<string, List<ForeignKey>> tableForeignKeys in _TableForeignKeysMap)
            {
                string fileText = ReadEntityText(tableForeignKeys.Key);

                StringBuilder foreignKeys = new();
                foreach (ForeignKey foreignKey in tableForeignKeys.Value)
                {
                    foreach (string definingColumn in foreignKey.DefiningColumns)
                    {
                        foreignKeys.AppendLine($"\t\t[ForeignKey(\"{definingColumn}\")]");
                    }
                    foreignKeys.AppendLine($"\t\tpublic virtual ICollection<{foreignKey.DefiningTable}> {foreignKey.DefiningTable} {{ get; set; }}");
                }
                fileText = fileText.Replace("//ForeignKeysForeign", foreignKeys.ToString());

                WriteFile($"{_SaveLocation}\\Entities", $"{tableForeignKeys.Key}.cs", fileText);
            }
        }

        private void GenerateExtension()
        {
            string templateText = ReadTemplateText("Extensions\\ContextNameDomainExtension.cs");
            templateText = templateText.Replace("ContextName", Context.Name);

            StringBuilder scopedServices = new();
            foreach (string service in _ScopedServices)
            {
                scopedServices.AppendLine(service);
            }
            templateText = templateText.Replace("//ScopedServices", scopedServices.ToString());

            WriteFile($"{_SaveLocation}\\Extensions", $"{Context.Name}DomainExtension.cs", templateText);
        }

        private void GenerateEntity(Table table)
        {
            if (!_TableForeignKeysMap.ContainsKey(table.Name))
                _TableForeignKeysMap.Add(table.Name, new List<ForeignKey>());

            string templateText = ReadTemplateText("Entities\\TableName.cs");
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("TableName", table.Name);

            StringBuilder columnBuilder = new();
            foreach (Column column in table.Columns)
            {
                string nullable = column.Nullable || column.Identity ? "?" : "";

                foreach (ForeignKey foreignKey in table.ForeignKeys.Where(key => key.DefiningColumns.Contains(column.Name)))
                {
                    columnBuilder.AppendLine($"\t\t[ForeignKey(\"{foreignKey.ForeignTable}\")]");
                }

                columnBuilder.AppendLine($"\t\tpublic {column.DataType.ToString()}{nullable} {column.Name} {{ get; set; }}");
            }
            templateText = templateText.Replace("//Columns", columnBuilder.ToString());

            StringBuilder foreignKeys = new();
            foreach (ForeignKey foreignKey in table.ForeignKeys)
            {
                if (!_TableForeignKeysMap.ContainsKey(foreignKey.ForeignTable))
                    _TableForeignKeysMap.Add(foreignKey.ForeignTable, new List<ForeignKey>());

                _TableForeignKeysMap[foreignKey.ForeignTable].Add(foreignKey);
                foreignKeys.AppendLine($"\t\tpublic virtual ICollection<{foreignKey.ForeignTable}> {foreignKey.ForeignTable} {{ get; set; }}");
            }
            templateText = templateText.Replace("//ForeignKeysDefinition", foreignKeys.ToString());

            WriteFile($"{_SaveLocation}\\Entities", $"{table.Name}.cs", templateText);
        }

        private void GenerateLookupRequest(Table table)
        {
            string templateText = ReadTemplateText("Models\\LookupRequests\\TableNameLookupRequest.cs");
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("TableName", table.Name);

            StringBuilder columnBuilder = new();
            foreach (Column column in table.Columns)
            {
                columnBuilder.AppendLine($"\t\tpublic {column.DataType.ToString()}? {column.Name} {{ get; set; }}");
            }
            templateText = templateText.Replace("//Columns", columnBuilder.ToString());

            WriteFile($"{_SaveLocation}\\Models\\LookupRequests", $"{table.Name}LookupRequest.cs", templateText);
        }

        private void GenerateService(Table table)
        {
            string templateText = ReadTemplateText("Services\\TableNameService.cs");
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("TableName", table.Name);

            WriteFile($"{_SaveLocation}\\Services", $"{table.Name}Service.cs", templateText);
            _ScopedServices.Add($"\t\t\tservices.AddScoped<I{table.Name}Service, {table.Name}Service>();");
        }
    }
}
