using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Repositories;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.Persistence;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.Responses;
using MassTransit;
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


    public async Task<LinkClick?> GetClickByIdAsync(Guid id)
    {
        return await _context.LinkClicks.FindAsync(id);
    }

    public async Task<IEnumerable<LinkClick>> GetClicksByShortCodeAsync(string shortCode)
    {
        var linkId = await _context.ShortenedLinks
            .Where(l => l.ShortCode == shortCode)
            .Select(l => l.Id)
            .FirstOrDefaultAsync();

        if (linkId == Guid.Empty)
        {
            return Enumerable.Empty<LinkClick>();
        }

        // 2. Get Clicks
        return await _context.LinkClicks
            .Where(c => c.ShortenedLinkId == linkId)
            .OrderByDescending(c => c.ClickedAt)
            .ToListAsync();
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

    public async Task<IEnumerable<ShortenedLink>> GetAllByUserIdAsync(Guid userId)
    {
        return await _context.ShortenedLinks
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<DashboardLinkDto>> GetDashboardLinksAsync(Guid userId)
    {
        return await _context.ShortenedLinks
        .Where(link => link.UserId == userId)
        .Select(link => new
        {
            CampaignName = link.CampaignName,
            OriginalUrl = link.OriginalUrl,
            ShortCode = link.ShortCode,
            TotalClicks = _context.LinkClicks.Count(click => click.ShortenedLinkId == link.Id)
        })
        .OrderByDescending(x => x.TotalClicks)
        .Select(x => new DashboardLinkDto(
            x.CampaignName ?? "Sem Campanha",
            x.OriginalUrl,
            x.ShortCode,
            x.TotalClicks
        ))
        .ToListAsync();
    }
}
