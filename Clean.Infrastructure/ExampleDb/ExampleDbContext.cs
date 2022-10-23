using Clean.Domain.Example;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Clean.Infrastructure.ExampleDb
{
    public class ExampleDbContext : DbContext
    {
        private readonly string _connectionString = string.Empty;

        public ExampleDbContext() { }

        public ExampleDbContext(IConfiguration configuration)
        {
            if (configuration["ExampleDbConnectionString"] == null)
                throw new ArgumentNullException(nameof(configuration));

            _connectionString = configuration["ExampleDbConnectionString"];
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public DbSet<Example> Examples { get; set; }
    }
}