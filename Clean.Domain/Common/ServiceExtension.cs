using Microsoft.Extensions.DependencyInjection;
using Clean.Infrastructure.SQL;
using Clean.Domain.SQL;
using Clean.Domain.Common.Interfaces;

namespace Clean.Domain.Common
{
    public static class ServiceExtension
    {
        public static void SQLInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IDbContextFactory, DbContextFactory>();
            services.AddScoped(typeof(IRepository<>), typeof(SQLRepository<>));
        }
    }
}