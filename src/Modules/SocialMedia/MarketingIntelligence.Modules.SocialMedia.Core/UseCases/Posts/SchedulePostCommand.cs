using System;
using MediatR;
using MarketingIntelligence.Shared.Results;

namespace MarketingIntelligence.Modules.SocialMedia.Core.UseCases.Posts;

public record SchedulePostCommand(
    Guid BrandId,
    string Content,
    DateTimeOffset ScheduledAt,
    string[] Platforms
) : IRequest<Result<Guid>>;