namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

public record InitialCommand(
    Guid ResellerId,
    string ResellerCode,
    Guid LicenseeId,
    string LicenseeCode,
    Guid BrandId,
    string BrandCode,
    DateTimeOffset Month);
