//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//
//Last Generated: 12/29/2022 03:37//

using Clean.Domain.Common.Interfaces;
using Clean.Domain.ExampleContext.Entities;
using Clean.Infrastructure.SQL;
using Microsoft.Extensions.DependencyInjection;

namespace Clean.Infrastructure.ExampleContext
{
    public static class ExampleInfrastructureExtension
    {
        public static void ExampleInfrastructure(this IServiceCollection services)
        {
			services.AddScoped<IRepository<Examples>, SQLRepository<ExampleDbContext, Examples>>();
			services.AddScoped<IReadOnlyRepository<Examples>, SQLRepository<ExampleDbContext, Examples>>();
			services.AddScoped<IRepository<Samples>, SQLRepository<ExampleDbContext, Samples>>();
			services.AddScoped<IReadOnlyRepository<Samples>, SQLRepository<ExampleDbContext, Samples>>();

        }
    }
}