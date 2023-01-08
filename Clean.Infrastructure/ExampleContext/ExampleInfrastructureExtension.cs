//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

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
			services.AddScoped<IRepository<Drivers>, SQLRepository<ExampleDbContext, Drivers>>();
			services.AddScoped<IRepository<Locations>, SQLRepository<ExampleDbContext, Locations>>();
			services.AddScoped<IRepository<Vehicles>, SQLRepository<ExampleDbContext, Vehicles>>();

        }
    }
}