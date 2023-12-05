namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

public record MonthCommand(
    Guid ResellerId,
    string ResellerCode,
    Guid LicenseeId,
    string LicenseeCode,
    Guid BrandId,
    string BrandCode);
