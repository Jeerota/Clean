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
            services.AddScoped<IRepository<Example>, SQLRepository<ExampleDbContext, Example>>();
            services.AddScoped<IReadOnlyRepository<Example>, SQLRepository<ExampleDbContext, Example>>();
            services.AddScoped<IRepository<Sample>, SQLRepository<ExampleDbContext, Sample>>();
            services.AddScoped<IReadOnlyRepository<Sample>, SQLRepository<ExampleDbContext, Sample>>();
        }
    }
}