using MarketingIntelligence.Modules.Identity.Core.Identity.Entities;
using MarketingIntelligence.Modules.Identity.Core.Shared;
using MarketingIntelligence.Modules.Identity.Core.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketingIntelligence.Modules.Identity.Infrastructure.Persistence
{
    public class IdentityDbContext : DbContext, IUnitOfWork
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {   
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserCredential> UserCredentials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("identity");

            modelBuilder.Entity<UserCredential>(entity =>
            {
                entity.ToTable("credentials");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
            });
        }
    }
}
