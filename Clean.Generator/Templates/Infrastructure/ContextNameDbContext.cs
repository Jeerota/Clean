//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Infrastructure.ContextNameContext.Entities;
using Clean.Infrastructure.ContextNameContext.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
//RefTableName
namespace Clean.Infrastructure.ContextNameContext
{
    public class ContextNameDbContext : DbContext
    {
        private readonly string _connectionString = string.Empty;

        public ContextNameDbContext()
        {
            this.ChangeTracker.AutoDetectChangesEnabled = false;
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public ContextNameDbContext(IConfiguration configuration)
        {
            if (configuration["ContextNameDbConnectionString"] == null)
                throw new ArgumentNullException(nameof(configuration));
            else
                _connectionString = configuration["ContextNameDbConnectionString"];
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
//ApplyConfigurations
            base.OnModelCreating(modelBuilder);
        }

//Tables
    }
}