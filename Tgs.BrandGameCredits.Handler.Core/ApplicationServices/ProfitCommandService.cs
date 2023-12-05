using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class ProfitCommandService(
    IBrandGameCreditTimeUtnsil timeUtnsil,
    IGameWagerStatDailyCloseRepository gameWagerStatDailyCloseRepository,
    IBrandGameCreditRepository brandGameCreditRepository) : ICommandService<ProfitCommand>
{
    public async ValueTask ExecuteAsync(ProfitCommand command, CancellationToken cancellationToken = default)
    {
        var month = timeUtnsil.GetDateOnly(command.Month, 480);

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
