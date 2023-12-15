using Tgs.BrandGameCredits.Handler.Core;
using Tgs.BrandGameCredits.Handler.Core.Models;
using Tgs.BrandGameCredits.Handler.Repositories.Entities;
using Z.EntityFramework.Plus;

namespace Tgs.BrandGameCredits.Handler.Repositories;

internal class BrandGameCreditProfitRepository(TgsContext context) : IBrandGameCreditProfitRepository
{
    public async ValueTask DeleteBatchAsync(Guid brandId, DateOnly month, CancellationToken cancellationToken = default)
    {
        _ = await context.BrandGameCreditProfits
            .Where(predicate: predicate => predicate.BrandId == brandId && predicate.Month == month)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask InsertBatchAsync(IEnumerable<BrandGameCreditProfitAdd> source, CancellationToken cancellationToken = default)
    {
        if (!source.Any())
        {
            return;
        }

        var entities = source.Select(selector: selector => new BrandGameCreditProfit
        {
            BrandId = selector.BrandId,
            Month = selector.Month,
            GameProviderId = selector.GameProviderId,
            Count = selector.Count,
            Bet = selector.Bet,
            ValidBet = selector.ValidBet,
            Settle = selector.Settle,
            Commission = selector.Commission,
            Turnover = selector.Turnover,
            WinLoss = selector.WinLoss,
        });

        await context.BrandGameCreditProfits.AddRangeAsync(
            entities,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = await context.SaveChangesAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
