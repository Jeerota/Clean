using Clean.Generator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Generator
{
    public class DomainGenerator
    {
        private const string _TemplateDirectory = "C:\\Users\\Justin\\Source\\Repos\\Clean.Infrastructure\\Clean.Generator\\Templates\\Domain";

        private Context Context;
        private string _SaveLocation;
        private StringBuilder? _ColumnBuilder;
        private List<string> _ScopedServices;

        public DomainGenerator(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _SaveLocation = $"C:\\temp\\Domain\\{Context.Name}Context";

            _ScopedServices = new();
            foreach (Table table in context.Tables)
            {
                _ColumnBuilder = new();
                GenerateEntity(table);
                GenerateLookupRequest(table);
                GenerateService(table);
            }

            GenerateExtension();
        }

        private static string ReadTemplateText(string templateLocation)
        {
            StreamReader templateReader = new($"{_TemplateDirectory}\\{templateLocation}");
            return templateReader.ReadToEnd();
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
            if (_ColumnBuilder == null)
                throw new(nameof(_ColumnBuilder));

            string templateText = ReadTemplateText("Entities\\TableName.cs");
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("TableName", table.Name);

            foreach (Column column in table.Columns)
            {
                string dataType = "string"; //ToDo: Get data type of column
                _ColumnBuilder.AppendLine($"\t\tpublic {dataType} {column.Name} {{ get; set; }}");
            }
            templateText = templateText.Replace("//Columns", _ColumnBuilder.ToString());

            WriteFile($"{_SaveLocation}\\Entities", $"{table.Name}.cs", templateText);
        }

        private void GenerateLookupRequest(Table table)
        {
            if (_ColumnBuilder == null)
                throw new(nameof(_ColumnBuilder));

            string templateText = ReadTemplateText("Models\\LookupRequests\\TableNameLookupRequest.cs");
            templateText = templateText.Replace("ContextName", Context.Name);
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
            templateText = templateText.Replace("TableName", table.Name);

            WriteFile($"{_SaveLocation}\\Services", $"{table.Name}Service.cs", templateText);
            _ScopedServices.Add($"\t\t\tservices.AddScoped<I{table.Name}Service, {table.Name}Service>();");
        }
    }
}
