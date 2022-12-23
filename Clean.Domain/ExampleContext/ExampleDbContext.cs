using Clean.Domain.ExampleContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Clean.Domain.ExampleContext
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
        public DbSet<Sample> Samples { get; set; }
    }
}