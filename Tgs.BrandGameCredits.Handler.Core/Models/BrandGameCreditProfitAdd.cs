namespace Tgs.BrandGameCredits.Handler.Core.Models;

public record BrandGameCreditProfitAdd(
	Guid BrandId,
	DateOnly Month,
	Guid GameProviderId,
	int Count,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal Commission,
	decimal Turnover,
	decimal WinLoss,
	decimal ProfitAmount);
