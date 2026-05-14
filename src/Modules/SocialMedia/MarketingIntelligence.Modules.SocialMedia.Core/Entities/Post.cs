using System;

namespace MarketingIntelligence.Modules.SocialMedia.Core.Entities;

public class Post
{
    public Guid Id { get; private set; }
    public Guid BrandId { get; private set; }
    public string Content { get; private set; }
    public DateTimeOffset ScheduledAt { get; private set; }
    public string[] Platforms { get; private set; }

    public Post(Guid brandId, string content, DateTimeOffset scheduledAt, string[] platforms)
    {
        Id = Guid.NewGuid();
        BrandId = brandId;
        Content = content;
        ScheduledAt = scheduledAt;
        Platforms = platforms;
    }
}