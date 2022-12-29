using Clean.Generator.Models;
using System.Text;

namespace Clean.Generator
{
    public class InfrastructureGenerator
    {
        private const string _TemplateDirectory = "C:\\Users\\Justin\\Source\\Repos\\Clean.Infrastructure\\Clean.Generator\\Templates\\Infrastructure";

        private Context Context;
        private string _SaveLocation;
        private DateTime _GenerationTime;

        public InfrastructureGenerator(Context context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _SaveLocation = $"C:\\temp\\Infrastructure\\{Context.Name}Context";
            _GenerationTime = DateTime.UtcNow;

            GenerateDbContext();
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

        private void GenerateDbContext()
        {
            string templateText = ReadTemplateText("ContextNameDbContext.cs");
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("GeneratedDateTimeStamp", _GenerationTime.ToShortDateString() + " " + _GenerationTime.ToShortTimeString());

            StringBuilder tables = new();
            foreach (Table table in Context.Tables)
            {
                tables.AppendLine($"\t\tpublic DbSet<{table.Name}>? {table.Name} {{ get; set; }}");
            }
            templateText = templateText.Replace("//Tables", tables.ToString());

             
            WriteFile($"{_SaveLocation}", $"{Context.Name}DbContext.cs", templateText);
        }

        private void GenerateExtension()
        {
            string templateText = ReadTemplateText("ContextNameInfrastructureExtension.cs");
            templateText = templateText.Replace("ContextName", Context.Name);
            templateText = templateText.Replace("GeneratedDateTimeStamp", _GenerationTime.ToShortDateString() + " " + _GenerationTime.ToShortTimeString());

            StringBuilder scopedRepositories = new();
            foreach (Table table in Context.Tables)
            {
                scopedRepositories.AppendLine($"\t\t\tservices.AddScoped<IRepository<{table.Name}>, SQLRepository<{Context.Name}DbContext, {table.Name}>>();");
                scopedRepositories.AppendLine($"\t\t\tservices.AddScoped<IReadOnlyRepository<{table.Name}>, SQLRepository<{Context.Name}DbContext, {table.Name}>>();"); //ToDo: May only need to call Full Repo for extensions.
            }
            templateText = templateText.Replace("//ScopedRepositories", scopedRepositories.ToString());

            WriteFile($"{_SaveLocation}", $"{Context.Name}InfrastructureExtension.cs", templateText);
        }
    }
}
