using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketingIntelligence.Modules.LinkShortener.Infrastructure.Persistence;

public class LinkShortenerDbContext : DbContext
{
    public LinkShortenerDbContext(DbContextOptions<LinkShortenerDbContext> options) : base(options)
    {
    }

    public DbSet<ShortenedLink> ShortenedLinks { get; set; }
    public DbSet<LinkClick> LinkClicks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("link_shortener");

        modelBuilder.Entity<ShortenedLink>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ShortCode).IsUnique();
            entity.HasIndex(e => e.OriginalUrl);
            
            // Sequence for ID generation
            entity.Property(e => e.SequenceId)
                  .UseSequence("ShortenedLinkSequence");
        });

        modelBuilder.Entity<LinkClick>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ShortenedLinkId);
            entity.HasIndex(e => e.ClickedAt);
        });
        
        // Sequence definition
        modelBuilder.HasSequence<long>("ShortenedLinkSequence")
                    .StartsAt(1)
                    .IncrementsBy(1);
    }
}
