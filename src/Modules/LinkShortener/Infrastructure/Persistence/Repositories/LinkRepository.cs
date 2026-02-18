using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Repositories;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MarketingIntelligence.Modules.LinkShortener.Infrastructure.Persistence.Repositories;

public class LinkRepository : ILinkRepository
{
    private readonly LinkShortenerDbContext _context;

    public LinkRepository(LinkShortenerDbContext context)
    {
        _context = context;
    }

    public async Task<ShortenedLink?> GetByShortCodeAsync(string shortCode)
    {
        return await _context.ShortenedLinks
            .FirstOrDefaultAsync(l => l.ShortCode == shortCode);
    }

    public async Task<ShortenedLink?> GetByOriginalUrlAsync(string originalUrl)
    {
        return await _context.ShortenedLinks
            .FirstOrDefaultAsync(l => l.OriginalUrl == originalUrl);
    }

    public async Task AddAsync(ShortenedLink link)
    {
        await _context.ShortenedLinks.AddAsync(link);
    }

    public async Task AddClickAsync(LinkClick click)
    {
        await _context.LinkClicks.AddAsync(click);
    }

    public async Task<long> GetNextSequenceIdAsync()
    {
        var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT nextval('link_shortener.\"ShortenedLinkSequence\"')";
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt64(result);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
