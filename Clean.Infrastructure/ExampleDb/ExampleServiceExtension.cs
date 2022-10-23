using Clean.Domain.Example;
using Microsoft.Extensions.DependencyInjection;
using Clean.Infrastructure.SQL;
using Clean.Application.Common;

namespace Clean.Infrastructure.ExampleDb
{
    public static class ExampleServiceExtension
    {
        public static void ExampleServices(this IServiceCollection services)
        {
            services.AddScoped<IRepository<ExampleDbContext, Example>, SQLRepository<ExampleDbContext, Example>>();
            services.AddScoped<IReadOnlyRepository<ExampleDbContext, Example>, SQLRepository<ExampleDbContext, Example>>(); 
            services.AddScoped<IRepository<ExampleDbContext, Sample>, SQLRepository<ExampleDbContext, Sample>>();
            services.AddScoped<IReadOnlyRepository<ExampleDbContext, Sample>, SQLRepository<ExampleDbContext, Sample>>();
        }
    }
}