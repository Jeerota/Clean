using Microsoft.Extensions.DependencyInjection;

namespace Clean.Infrastructure.SQL
{
    public static class SQLServiceExtension
    {
        public static void SQLInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IDbContextFactory, DbContextFactory>();
        }
    }
}