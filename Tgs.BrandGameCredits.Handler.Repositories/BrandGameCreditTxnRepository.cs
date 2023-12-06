using Microsoft.EntityFrameworkCore;
using Tgs.BrandGameCredits.Handler.Core;
using Tgs.BrandGameCredits.Handler.Repositories.Entities;

namespace Tgs.BrandGameCredits.Handler.Repositories;

internal class BrandGameCreditTxnRepository(TgsContext context) : IBrandGameCreditTxnRepository
{
    public async ValueTask<decimal> SumCreditAsync(Guid brandId, DateTimeOffset createdOnStart, DateTimeOffset createdOnEnd, CancellationToken cancellationToken)
        => await context.BrandGameCreditTxns.AsQueryable()
        .Where(
            predicate: predicate => predicate.CreatedOn >= createdOnStart.UtcDateTime
            && predicate.CreatedOn < createdOnEnd.UtcDateTime
            && predicate.BrandId == brandId
            && predicate.Type == 1)
        .SumAsync(
            selector: selector => selector.Amount,
            cancellationToken)
        .ConfigureAwait(false);

    public async ValueTask<decimal> SumDebitAsync(Guid brandId, DateTimeOffset createdOnStart, DateTimeOffset createdOnEnd, CancellationToken cancellationToken)
        => await context.BrandGameCreditTxns.AsQueryable()
        .Where(
            predicate: predicate => predicate.CreatedOn >= createdOnStart.UtcDateTime
            && predicate.CreatedOn < createdOnEnd.UtcDateTime
            && predicate.BrandId == brandId
            && predicate.Type == 2)
        .SumAsync(
            selector: selector => selector.Amount,
            cancellationToken)
        .ConfigureAwait(false);
}
