using Tgs.BrandGameCredits.Handler.Common;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

public record UpdatePublishCommand(
    Guid ResellerId,
    string ResellerCode,
    Guid LicenseeId,
    string LicenseeCode,
    Guid BrandId,
    string BrandCode);