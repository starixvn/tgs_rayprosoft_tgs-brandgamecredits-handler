using Microsoft.EntityFrameworkCore;
using Tgs.BrandGameCredits.Handler.Core;
using Tgs.BrandGameCredits.Handler.Core.Models;
using Tgs.BrandGameCredits.Handler.Repositories.Entities;

namespace Tgs.BrandGameCredits.Handler.Repositories;

internal class GameWagerStatDailyCloseRepository(TgsContext context) : IGameWagerStatDailyCloseRepository
{
    public IAsyncEnumerable<GameWagerStatDailyCloseInfo> QueryAsync(Guid brandId, DateTimeOffset statOnStart, DateTimeOffset statOnEnd, DateOnly dayStart, DateOnly dayEnd, CancellationToken cancellationToken = default)
        => context.GameWagerStatDailyCloses.AsQueryable()
        .Where(predicate => predicate.StatOn >= statOnStart.UtcDateTime
        && predicate.StatOn < statOnEnd.UtcDateTime
        && predicate.StatDayOn >= dayStart
        && predicate.StatDayOn < dayEnd
        && predicate.BrandId == brandId)
        .GroupBy(keySelector => new
        {
            keySelector.ResellerId,
            keySelector.LicenseeId,
            keySelector.BrandId,
            keySelector.GameProviderId
        }).Select(selector => new GameWagerStatDailyCloseInfo(
            selector.Key.ResellerId,
            selector.Key.LicenseeId,
            selector.Key.BrandId,
            selector.Key.GameProviderId,
            dayStart,
            dayEnd.AddDays(-1),
            selector.Sum(selector => selector.Count),
            selector.Sum(selector => selector.Bet),
            selector.Sum(selector => selector.ValidBet),
            selector.Sum(selector => selector.Settle),
            selector.Sum(selector => selector.Commission),
            selector.Sum(selector => selector.Turnover),
            selector.Sum(selector => selector.WinLoss)
            )
       ).AsAsyncEnumerable();
}
