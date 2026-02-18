using MarketingIntelligence.Shared;


namespace MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities
{
    public class ShortenedLink : Entity
    {
        public string OriginalUrl { get; private set; }
        public string ShortCode { get; private set; }
        public uint SequenceId { get; private set; }

        public ShortenedLink(string originalUrl, uint sequenceId, string shortCode)
        {
            OriginalUrl = originalUrl;
            SequenceId = sequenceId;
            ShortCode = shortCode;
        }
    }
}
