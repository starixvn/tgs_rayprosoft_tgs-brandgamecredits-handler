using Tgs.BrandGameCredits.Handler.Core.Models;

namespace Tgs.BrandGameCredits.Handler.Core;

public interface IBrandGameCreditProfitRepository
{
    ValueTask InsertBatchAsync(IEnumerable<BrandGameCreditProfitAdd> source, CancellationToken cancellationToken = default);

    ValueTask DeleteBatchAsync(Guid brandId, DateOnly month, CancellationToken cancellationToken = default);
}
