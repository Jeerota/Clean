using Microsoft.Extensions.DependencyInjection;
using Clean.Infrastructure.SQL;
using Clean.Application.Common;
using Clean.Domain.ExampleContext.Entities;
using Clean.Domain.ExampleContext.Services;

namespace Clean.Domain.ExampleContext
{
    public static class ExampleDomainExtension
    {
        public static void ExampleDomain(this IServiceCollection services)
        {
            services.AddScoped<IRepository<ExampleDbContext, Example>, SQLRepository<ExampleDbContext, Example>>();
            services.AddScoped<IReadOnlyRepository<ExampleDbContext, Example>, SQLRepository<ExampleDbContext, Example>>(); 
            services.AddScoped<IRepository<ExampleDbContext, Sample>, SQLRepository<ExampleDbContext, Sample>>();
            services.AddScoped<IReadOnlyRepository<ExampleDbContext, Sample>, SQLRepository<ExampleDbContext, Sample>>();

            services.AddScoped<IExampleService, ExampleService>();
            services.AddScoped<ISampleService, SampleService>();
        }
    }
}