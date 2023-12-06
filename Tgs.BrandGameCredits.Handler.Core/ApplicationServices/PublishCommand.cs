using Tgs.BrandGameCredits.Handler.Common;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

public record PublishCommand(
    string Subject,
    Guid ResellerId,
    string ResellerCode,
    Guid LicenseeId,
    string LicenseeCode,
    Guid BrandId,
    string BrandCode,
    DateTimeOffset CurrentTime);