using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Clean.Infrastructure.SQL
{
    public interface IDbContextFactory
    {
        DbContext GetDbContext<TContext>() where TContext : DbContext, new();
    }

    public class DbContextFactory : IDbContextFactory
    {
        private readonly IConfiguration _configuration;

        public DbContextFactory(IConfiguration configuration)
        {
            if(configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            else
                _configuration = configuration;
        }

        public DbContext GetDbContext<TContext>() where TContext : DbContext, new() 
            => (TContext?)Activator.CreateInstance(typeof(TContext), new object[] { _configuration }) ?? throw new ArgumentNullException("dbContext");
    }
}