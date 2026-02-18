using MarketingIntelligence.Modules.Customers.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketingIntelligence.Modules.Customers.Infrastructure.Persistence
{
    public class CustomersDbContext : DbContext
    {
        public CustomersDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Customer> Customers => Set<Customer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("customers");

            modelBuilder.Entity<Customer>(builder =>
            {
                builder.ToTable("Customers");
                builder.HasKey(c => c.Id);

                builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
                builder.Property(c => c.Email).IsRequired().HasMaxLength(255);
                builder.Property(c => c.TaxId).IsRequired().HasMaxLength(20);

                builder.OwnsOne(c => c.BrandIdentity, nav =>
                {
                    nav.ToJson(); 
                });
            });
        }
    }
}
