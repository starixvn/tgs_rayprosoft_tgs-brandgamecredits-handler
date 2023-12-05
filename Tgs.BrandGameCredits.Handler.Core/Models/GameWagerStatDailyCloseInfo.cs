namespace Tgs.BrandGameCredits.Handler.Core.Models;

public record GameWagerStatDailyCloseInfo(
    Guid ResellerId,
    Guid LicenseeId,
    Guid BrandId,
    Guid GameProviderId,
    DateOnly DayStart,
    DateOnly DayEnd,
    int Count,
    decimal Bet,
    decimal ValidBet,
    decimal Settle,
    decimal Commission,
    decimal Turnover,
    decimal WinLoss);
