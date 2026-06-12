using MarketingIntelligence.Modules.Customers.Core.Domain.Entities;
using MarketingIntelligence.Modules.Customers.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MarketingIntelligence.Modules.Customers.Infrastructure.Persistence;

public class CustomersDbContext : DbContext
{
    public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();
    public DbSet<CustomerContact> CustomerContacts => Set<CustomerContact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("customers");

        modelBuilder.Entity<Customer>(builder =>
        {
            builder.ToTable("Customers");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(250);
            builder.Property(c => c.Email).IsRequired().HasMaxLength(255);
            builder.Property(c => c.Phone).IsRequired().HasMaxLength(30);
            builder.Property(c => c.TaxId).IsRequired().HasMaxLength(20);
            builder.Property(c => c.Type)
                .IsRequired()
                .HasConversion<int>();
            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<int>();
            builder.Property(c => c.Notes).HasMaxLength(2000);
            builder.Property(c => c.UserId).IsRequired();
            builder.Property(c => c.TradingName).HasMaxLength(250);
            builder.Property(c => c.StateRegistration).HasMaxLength(30);
            builder.Property(c => c.Gender).HasMaxLength(20);

            builder.HasIndex(c => c.Email);
            builder.HasIndex(c => c.TaxId).IsUnique();
            builder.HasIndex(c => c.UserId);

            // BrandIdentity mapped as JSON column
            builder.OwnsOne(c => c.BrandIdentity, nav =>
            {
                nav.ToJson();
            });

            // Addresses navigation
            builder.HasMany(c => c.Addresses)
                .WithOne()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Contacts navigation
            builder.HasMany(c => c.Contacts)
                .WithOne()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Discriminator for future TPH if needed
            // (Currently using CustomerType enum on the same table)
        });

        modelBuilder.Entity<CustomerAddress>(builder =>
        {
            builder.ToTable("CustomerAddresses");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Street).IsRequired().HasMaxLength(250);
            builder.Property(a => a.Number).IsRequired().HasMaxLength(20);
            builder.Property(a => a.Complement).HasMaxLength(250);
            builder.Property(a => a.Neighborhood).IsRequired().HasMaxLength(150);
            builder.Property(a => a.City).IsRequired().HasMaxLength(150);
            builder.Property(a => a.State).IsRequired().HasMaxLength(50);
            builder.Property(a => a.ZipCode).IsRequired().HasMaxLength(10);
            builder.Property(a => a.Label).HasMaxLength(50);

            builder.HasIndex(a => a.CustomerId);
        });

        modelBuilder.Entity<CustomerContact>(builder =>
        {
            builder.ToTable("CustomerContacts");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Email).IsRequired().HasMaxLength(255);
            builder.Property(c => c.Phone).HasMaxLength(30);
            builder.Property(c => c.Role).HasMaxLength(100);

            builder.HasIndex(c => c.CustomerId);
        });
    }
}
