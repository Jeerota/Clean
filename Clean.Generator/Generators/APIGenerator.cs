using Clean.Generator.Helpers;
using Clean.Generator.Models;
using Microsoft.Extensions.Configuration;

namespace Clean.Generator.Generators
{
    public class APIGenerator
    {
        private readonly string _TemplateDirectory = $"{AppContext.BaseDirectory.Split("\\bin")[0]}\\Templates\\API";
        private readonly string _DefaultNamespace;
        private readonly string _OutputDirectory;
        private readonly bool _IncludeSchemaLabel;

        private Context Context;
        private string _SaveLocation;

        public APIGenerator(IConfiguration config, Context context)
        {
            _IncludeSchemaLabel = bool.TryParse(config["Values:IncludeSchemaLabel"], out bool includeSchemaLabel) ? includeSchemaLabel : false;
            _DefaultNamespace = config["Values:DefaultNamespace"];
            _OutputDirectory = config["Values:APIOutputDirectory"];

            Context = context ?? throw new ArgumentNullException(nameof(context));
            _SaveLocation = $"{_OutputDirectory}\\{Context.Name}Context";

            List<string> functionTemplateNames = new() { "CreateTableName", "GetTableName", "DeleteTableName", "UpdateTableName" };
            HashSet<string> apiTypes = config["Values:APITypes"].Split(",").ToHashSet();

            foreach (Table table in context.Tables)
            {
                if (apiTypes.Contains(APITypes.Function.ToString()))
                    foreach (string templateName in functionTemplateNames)
                        GenerateFunction(table, templateName);

                if (apiTypes.Contains(APITypes.Controller.ToString()))
                    GenerateController(table);
            }
        }

        private void GenerateFunction(Table table, string templateName)
        {
            string tableName = GeneratorExtensions.GetTableName(_IncludeSchemaLabel, table);
            string templateText = FileEditor.ReadTemplateText(_TemplateDirectory, $"Functions\\{templateName}.cs");
            templateText = templateText.Replace("Clean.", _DefaultNamespace);
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("TableName", tableName);

            FileEditor.WriteFile($"{_SaveLocation}\\Functions\\{tableName}", $"{templateName.Replace("TableName", tableName)}.cs", templateText);
        }

        private void GenerateController(Table table)
        {
            string tableName = GeneratorExtensions.GetTableName(_IncludeSchemaLabel, table);
            string templateText = FileEditor.ReadTemplateText(_TemplateDirectory, $"Controllers\\TableNameController.cs");
            templateText = templateText.Replace("Clean.", _DefaultNamespace);
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("TableName", tableName);

            FileEditor.WriteFile($"{_SaveLocation}\\Controllers", $"{tableName}Controller.cs", templateText);
        }
    }
}
