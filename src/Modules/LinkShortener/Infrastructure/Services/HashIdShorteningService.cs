using HashidsNet;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Services;

namespace MarketingIntelligence.Modules.LinkShortener.Infrastructure.Services;

public class HashIdShorteningService : IShorteningService
{
    private readonly Hashids _hashids;

    public HashIdShorteningService(string salt = "MarketingIntelligenceSalt", int minHashLength = 6)
    {
        // Alphabet for Base62: 0-9, a-z, A-Z
        string alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        _hashids = new Hashids(salt, minHashLength, alphabet);
    }

    public string Encode(long sequenceId)
    {
        // Hashids library works with long/int, so we cast uint to long
        return _hashids.EncodeLong(sequenceId);
    }

    public long Decode(string shortCode)
    {
        var decoded = _hashids.DecodeLong(shortCode);
        return decoded.Length > 0 ? decoded[0] : 0;
    }
}
