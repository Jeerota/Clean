using Clean.Generator;
using Clean.Generator.Models;
using Clean.Generator.Generators;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

ContextParser contextParser = new(configuration);

foreach(Context context in contextParser.Contexts)
{
    new DomainGenerator(configuration, context);
    new InfrastructureGenerator(configuration, context);
    new APIGenerator(configuration, context);
}