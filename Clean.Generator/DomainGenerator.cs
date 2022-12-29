using Clean.Generator.Models;
using System.Text;

namespace Clean.Generator
{
    public class DomainGenerator
    {
        private const string _TemplateDirectory = "C:\\Users\\Justin\\Source\\Repos\\Clean.Infrastructure\\Clean.Generator\\Templates\\Domain";

        private Context Context;
        private string _SaveLocation;
        private StringBuilder? _ColumnBuilder;
        private List<string> _ScopedServices;
        private Dictionary<string, List<ForeignKey>> _TableForeignKeysMap = new();
        private DateTime _GenerationTime;

        public DomainGenerator(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _SaveLocation = $"C:\\temp\\Domain\\{Context.Name}Context";
            _GenerationTime = DateTime.UtcNow;

            _ScopedServices = new();
            foreach (Table table in context.Tables)
            {
                _ColumnBuilder = new();
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
            templateText = templateText.Replace("GeneratedDateTimeStamp", _GenerationTime.ToShortDateString() + " " + _GenerationTime.ToShortTimeString());

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
            if (_ColumnBuilder == null)
                throw new(nameof(_ColumnBuilder));

            if (!_TableForeignKeysMap.ContainsKey(table.Name))
                _TableForeignKeysMap.Add(table.Name, new List<ForeignKey>());

            string templateText = ReadTemplateText("Entities\\TableName.cs");
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("GeneratedDateTimeStamp", _GenerationTime.ToShortDateString() + " " + _GenerationTime.ToShortTimeString());
            templateText = templateText.Replace("TableName", table.Name);

            foreach (Column column in table.Columns)
            {
                string nullable = column.Nullable ? "?" : "";

                foreach (ForeignKey foreignKey in table.ForeignKeys.Where(key => key.DefiningColumns.Contains(column.Name)))
                {
                    _ColumnBuilder.AppendLine($"\t\t[ForeignKey(\"{foreignKey.ForeignTable}\")]");
                }

                _ColumnBuilder.AppendLine($"\t\tpublic {column.DataType.ToString()}{nullable} {column.Name} {{ get; set; }}");
            }
            templateText = templateText.Replace("//Columns", _ColumnBuilder.ToString());

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
            if (_ColumnBuilder == null)
                throw new(nameof(_ColumnBuilder));

            string templateText = ReadTemplateText("Models\\LookupRequests\\TableNameLookupRequest.cs");
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("GeneratedDateTimeStamp", _GenerationTime.ToShortDateString() + " " + _GenerationTime.ToShortTimeString());
            templateText = templateText.Replace("TableName", table.Name);
            templateText = templateText.Replace("//Columns", _ColumnBuilder.ToString());

            WriteFile($"{_SaveLocation}\\Models\\LookupRequests", $"{table.Name}LookupRequest.cs", templateText);
        }

        private void GenerateService(Table table)
        {
            if (_ColumnBuilder == null)
                throw new(nameof(_ColumnBuilder));

            string templateText = ReadTemplateText("Services\\TableNameService.cs");
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("GeneratedDateTimeStamp", _GenerationTime.ToShortDateString() + " " + _GenerationTime.ToShortTimeString());
            templateText = templateText.Replace("TableName", table.Name);

            WriteFile($"{_SaveLocation}\\Services", $"{table.Name}Service.cs", templateText);
            _ScopedServices.Add($"\t\t\tservices.AddScoped<I{table.Name}Service, {table.Name}Service>();");
        }
    }
}
