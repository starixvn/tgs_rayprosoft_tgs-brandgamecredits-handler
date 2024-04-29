namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

public record CreditCommand(
	Guid ResellerId,
	string ResellerCode,
	Guid LicenseeId,
	string LicenseeCode,
	Guid BrandId,
	string BrandCode,
	DateTimeOffset CurrentTime);
