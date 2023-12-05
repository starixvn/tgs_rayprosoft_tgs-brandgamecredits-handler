using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class MonthlySettlementCommandService(
    ILogger<MonthlySettlementCommandService> logger,
    IBrandGameCreditTimeUtnsil timeUtnsil,
    IBrandGameCreditRepository brandGameCreditRepository,
    IGameWagerStatDailyCloseRepository gameWagerStatDailyCloseRepository) : ICommandService<MonthlySettlementCommand>
{
    public async ValueTask ExecuteAsync(MonthlySettlementCommand command, CancellationToken cancellationToken = default)
    {
        var month = timeUtnsil.GetDateOnly(command.Month, 480);

        logger.LogWarning("LogOn:{logOn} Message:This is used for monthly({month}) settlement of the brand({brand}).",
            DateTime.Now.ToString("o"),
            command.Month,
            command.BrandCode);

        var settlementBrandGameCredit = await brandGameCreditRepository.GetAsync(
            brandId: command.BrandId,
            month: month,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (settlementBrandGameCredit is null)
        {
            return;
        }

        var lastMonthBrandGameCredit = await brandGameCreditRepository.GetAsync(
            brandId: command.BrandId,
            month: month.AddMonths(-1),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var amount = 0.0m;

        if (lastMonthBrandGameCredit != null)
        {
            var credit = lastMonthBrandGameCredit.InitialAmount + lastMonthBrandGameCredit.CreditAmount - lastMonthBrandGameCredit.DebitAmount;
            amount = credit + lastMonthBrandGameCredit.ProfitAmount;
            if (amount > credit)
            {
                amount = credit;
            }
        }
        await brandGameCreditRepository.UpdateInitialAsync(
            brandId: command.BrandId,
            month: month,
            amount: amount,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await brandGameCreditRepository.UpdateCreditAsync(
            brandId: command.BrandId,
            month: month,
            amount: 0.0m,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await brandGameCreditRepository.UpdateDebitAsync(
            brandId: command.BrandId,
            month: month,
            amount: 0.0m,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameWagerStats = await gameWagerStatDailyCloseRepository.QueryAsync(
            brandId: command.BrandId,
            statOnStart: command.Month,
            statOnEnd: command.Month.AddMonths(1),
            dayStart: month,
            dayEnd: month.AddMonths(+1),
            cancellationToken: cancellationToken)
            .ToArrayAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await brandGameCreditRepository.UpdateProfitAsync(
            brandId: command.BrandId,
            month: month,
            amount: gameWagerStats.Sum(selector => selector.WinLoss),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
