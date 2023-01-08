using Clean.Generator;
using Clean.Generator.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Dac.Model;
using System.Data;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

ContextParser contextParser = new(configuration);

foreach(Context context in contextParser.Contexts)
{
    new DomainGenerator(configuration, context);
    new InfrastructureGenerator(configuration, context);
}