using Clean.Generator;
using Clean.Generator.Models;
using Clean.Generator.Generators;
using Microsoft.Extensions.Configuration;
using System.Text;
using Clean.Generator.Helpers;
using Clean.Infrastructure.ContextNameContext.Entities;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

ContextParser contextParser = new(configuration);

StringBuilder autoMapperConfig = new();
StringBuilder domainContexts = new();
StringBuilder infrastructureContexts = new();

foreach(Context context in contextParser.Contexts)
{
    domainContexts.AppendLine($"using {configuration["Values:DefaultNamespace"]}Domain.{context.Name}Context.Models.DTOs;");
    infrastructureContexts.AppendLine($"using {configuration["Values:DefaultNamespace"]}Infrastructure.{context.Name}Context.Entities;");
    new DomainGenerator(configuration, context);
    new InfrastructureGenerator(ref autoMapperConfig, configuration, context);
    new APIGenerator(configuration, context);
}

HashSet<string> infrastructureTypes = configuration["Values:InfrastructureTypes"].Split(",").ToHashSet();
if (infrastructureTypes.Contains(InfrastructureTypes.SQL.ToString()))
{
    string templateText = FileEditor.ReadTemplateText($"{AppContext.BaseDirectory.Split("\\bin")[0]}\\Templates\\Domain", "AutoMapperConfigurationHelper.cs");
    templateText = templateText.Replace("//DTOs", domainContexts.ToString());
    templateText = templateText.Replace("//Entities", infrastructureContexts.ToString());
    templateText = templateText.Replace("//EntityToDTOMaps", autoMapperConfig.ToString());

    FileEditor.WriteFile($"{configuration["Values:DomainOutputDirectory"]}\\SQL", $"AutoMapperConfigurationHelper.cs", templateText);
}
