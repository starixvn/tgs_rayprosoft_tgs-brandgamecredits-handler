using Tgs.BrandGameCredits.Handler.Common;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class InitialCommandService(
    IBrandGameCreditTimeUtnsil timeUtnsil,
    IBrandGameCreditRepository brandGameCreditRepository,
    ICommandService<InitialPublishCommand> initialPublishService) : ICommandService<InitialCommand>
{
    public async ValueTask ExecuteAsync(InitialCommand command, CancellationToken cancellationToken = default)
    {
        var monthTimeZone = timeUtnsil.Get1stOfEveryMonthTimeZone(command.CurrentTime, 480);
        var monthDateOnly = timeUtnsil.Get1stOfEveryMonthDateOnly(monthTimeZone, 480);

        var lastMonthBrandGameCredit = await brandGameCreditRepository.GetAsync(
            brandId: command.BrandId,
            month: monthDateOnly.AddMonths(-1),
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
            month: monthDateOnly,
            amount: amount,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await initialPublishService.ExecuteAsync(
            command: new InitialPublishCommand(
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: command.CurrentTime),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
