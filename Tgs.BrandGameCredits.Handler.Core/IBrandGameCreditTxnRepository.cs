namespace Tgs.BrandGameCredits.Handler.Core;

public interface IBrandGameCreditTxnRepository
{
	public ValueTask<decimal> SumCreditAsync(Guid brandId, DateTimeOffset createdOnStart, DateTimeOffset createdOnEnd, CancellationToken cancellationToken);

	public ValueTask<decimal> SumDebitAsync(Guid brandId, DateTimeOffset createdOnStart, DateTimeOffset createdOnEnd, CancellationToken cancellationToken);
}
