using Clean.Domain.ContextNameContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Clean.Infrastructure.ContextNameContext
{
    public class ContextNameDbContext : DbContext
    {
        private readonly string _connectionString = string.Empty;

        public ContextNameDbContext() { }

        public ContextNameDbContext(IConfiguration configuration)
        {
            if (configuration["ContextNameDbConnectionString"] == null)
                throw new ArgumentNullException(nameof(configuration));
            else
                _connectionString = configuration["ContextNameDbConnectionString"];
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

//Tables
    }
}