namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

public record StatCloseCommand(
    Guid ResellerId,
    string ResellerCode,
    Guid LicenseeId,
    string LicenseeCode,
    Guid BrandId,
    string BrandCode);
