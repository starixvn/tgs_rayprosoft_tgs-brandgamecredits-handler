namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

public record DebitCommand(
	Guid ResellerId,
	string ResellerCode,
	Guid LicenseeId,
	string LicenseeCode,
	Guid BrandId,
	string BrandCode,
	DateTimeOffset CurrentTime);
