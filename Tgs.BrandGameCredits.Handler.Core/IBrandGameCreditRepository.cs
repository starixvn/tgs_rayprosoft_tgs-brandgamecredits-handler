using Tgs.BrandGameCredits.Handler.Core.Models;

namespace Tgs.BrandGameCredits.Handler.Core;

public interface IBrandGameCreditRepository
{
    public ValueTask AddAsync(Guid resellerId, Guid licenseeId, Guid brandId, DateOnly month, CancellationToken cancellationToken = default);

    public ValueTask UpdateInitialAsync(Guid brandId, DateOnly month, decimal amount, CancellationToken cancellationToken = default);

    public ValueTask UpdateCreditAsync(Guid brandId, DateOnly month, decimal amount, CancellationToken cancellationToken = default);

    public ValueTask UpdateDebitAsync(Guid brandId, DateOnly month, decimal amount, CancellationToken cancellationToken = default);

    public ValueTask UpdateProfitAsync(Guid brandId, DateOnly month, decimal amount, CancellationToken cancellationToken = default);

    public ValueTask<BrandGameCreditInfo?> GetAsync(Guid brandId, DateOnly month, CancellationToken cancellationToken = default);
}
