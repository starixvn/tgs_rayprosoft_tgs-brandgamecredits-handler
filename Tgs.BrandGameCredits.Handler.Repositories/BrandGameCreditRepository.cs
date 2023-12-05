using Microsoft.EntityFrameworkCore;
using Tgs.BrandGameCredits.Handler.Core;
using Tgs.BrandGameCredits.Handler.Core.Models;
using Tgs.BrandGameCredits.Handler.Repositories.Entities;

namespace Tgs.BrandGameCredits.Handler.Repositories;

internal class BrandGameCreditRepository(TgsContext context) : IBrandGameCreditRepository
{
    public async ValueTask AddAsync(Guid resellerId, Guid licenseeId, Guid brandId, DateOnly month, CancellationToken cancellationToken = default)
    {
        var entity = new BrandGameCredit
        {
            ResellerId = resellerId,
            LicenseeId = licenseeId,
            BrandId = brandId,
            Month = month
        };

        _ = await context.AddAsync(
            entity: entity,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = await context.SaveChangesAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<BrandGameCreditInfo?> GetAsync(Guid brandId, DateOnly month, CancellationToken cancellationToken = default)
        => await context.BrandGameCredits
        .Where(predicate => predicate.Month == month && predicate.BrandId == brandId)
        .Select(selector => new BrandGameCreditInfo(
            selector.ResellerId,
            selector.LicenseeId,
            selector.BrandId,
            selector.Month,
            selector.InitialAmount,
            selector.CreditAmount,
            selector.DebitAmount,
            selector.ProfitAmount))
        .FirstOrDefaultAsync(
            cancellationToken: cancellationToken)
        .ConfigureAwait(false);

    public async ValueTask UpdateCreditAsync(Guid brandId, DateOnly month, decimal amount, CancellationToken cancellationToken = default)
    {
        var credit = await context.BrandGameCredits
            .SingleAsync(
            predicate => predicate.Month == month && predicate.BrandId == brandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        credit.CreditAmount = amount;
        credit.UpdatedOn = DateTime.UtcNow;

        _ = context.Update(credit);

        _ = await context.SaveChangesAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask UpdateDebitAsync(Guid brandId, DateOnly month, decimal amount, CancellationToken cancellationToken = default)
    {
        var credit = await context.BrandGameCredits
            .SingleAsync(
            predicate => predicate.Month == month && predicate.BrandId == brandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        credit.DebitAmount = amount;
        credit.UpdatedOn = DateTime.UtcNow;

        _ = context.Update(credit);

        _ = await context.SaveChangesAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask UpdateInitialAsync(Guid brandId, DateOnly month, decimal amount, CancellationToken cancellationToken = default)
    {
        var credit = await context.BrandGameCredits
            .SingleAsync(
            predicate => predicate.Month == month && predicate.BrandId == brandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        credit.InitialAmount = amount;
        credit.UpdatedOn = DateTime.UtcNow;

        _ = context.Update(credit);

        _ = await context.SaveChangesAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask UpdateProfitAsync(Guid brandId, DateOnly month, decimal amount, CancellationToken cancellationToken = default)
    {
        var credit = await context.BrandGameCredits
            .SingleAsync(
            predicate => predicate.Month == month && predicate.BrandId == brandId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        credit.ProfitAmount = amount;
        credit.UpdatedOn = DateTime.UtcNow;

        _ = context.Update(credit);

        _ = await context.SaveChangesAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
