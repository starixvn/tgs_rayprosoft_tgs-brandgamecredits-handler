namespace Tgs.BrandGameCredits.Handler.Core.Models;

public record BrandGameCreditInfo(
	 Guid ResellerId,
	 Guid LicenseeId,
	 Guid BrandId,
	 DateOnly Month,
	 decimal InitialAmount,
	 decimal CreditAmount,
	 decimal DebitAmount,
	 decimal ProfitAmount);
