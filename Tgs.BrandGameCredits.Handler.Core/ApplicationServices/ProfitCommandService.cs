using Tgs.BrandGameCredits.Handler.Common;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class ProfitCommandService(
    IBrandGameCreditTimeUtnsil timeUtnsil,
    IGameWagerStatDailyCloseRepository gameWagerStatDailyCloseRepository,
    IBrandGameCreditRepository brandGameCreditRepository,
    ICommandService<PublishCommand> publishService,
    ICommandService<UpdatePublishCommand> updatePublishService) : ICommandService<ProfitCommand>
{
    public async ValueTask ExecuteAsync(ProfitCommand command, CancellationToken cancellationToken = default)
    {
        var monthTimeZone = timeUtnsil.Get1stOfEveryMonthTimeZone(command.CurrentTime, 480);
        var monthDateOnly = timeUtnsil.Get1stOfEveryMonthDateOnly(monthTimeZone, 480);

        if (await brandGameCreditRepository.GetAsync(
            brandId: command.BrandId,
            month: monthDateOnly,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false) is null)
        {
            await publishService.ExecuteAsync(
                command: new PublishCommand(
                    Subject: BrandGameCreditsHandlerNatsConst.Settlement,
                    ResellerId: command.ResellerId,
                    ResellerCode: command.ResellerCode,
                    LicenseeId: command.LicenseeId,
                    LicenseeCode: command.LicenseeCode,
                    BrandId: command.BrandId,
                    BrandCode: command.BrandCode,
                    CurrentTime: monthTimeZone.AddMonths(-1)),
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return;
        }

        var gameWagerStats = await gameWagerStatDailyCloseRepository.QueryAsync(
            brandId: command.BrandId,
            statOnStart: monthTimeZone,
            statOnEnd: monthTimeZone.AddMonths(1),
            dayStart: monthDateOnly,
            dayEnd: monthDateOnly.AddMonths(+1),
            cancellationToken: cancellationToken)
            .ToArrayAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await brandGameCreditRepository.UpdateProfitAsync(
            brandId: command.BrandId,
            month: monthDateOnly,
            amount: gameWagerStats.Sum(selector => selector.WinLoss),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await updatePublishService.ExecuteAsync(
            command: new UpdatePublishCommand(
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
