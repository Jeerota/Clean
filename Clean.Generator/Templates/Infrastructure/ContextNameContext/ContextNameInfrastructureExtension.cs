using Clean.Domain.Common.Interfaces;
using Clean.Domain.ContextNameContext.Entities;
using Clean.Infrastructure.SQL;
using Microsoft.Extensions.DependencyInjection;

namespace Clean.Infrastructure.ContextNameContext
{
    public static class ContextNameInfrastructureExtension
    {
        public static void ContextNameInfrastructure(this IServiceCollection services)
        {
            //RepositoryScoped services.AddScoped<IRepository<TableName>, SQLRepository<ExampleDbContext, Template>>(); services.AddScoped<IReadOnlyRepository<TableName>, SQLRepository<ExampleDbContext, Template>>();
        }
    }
}