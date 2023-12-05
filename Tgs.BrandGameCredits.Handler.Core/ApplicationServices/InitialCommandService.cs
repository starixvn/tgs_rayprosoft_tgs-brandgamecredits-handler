using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.ServiceUtility;
using static System.Net.Mime.MediaTypeNames;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class InitialCommandService(
    IBrandGameCreditTimeUtnsil timeUtnsil,
    IBrandGameCreditRepository brandGameCreditRepository,
    ICommandService<PublishCommand> service) : ICommandService<InitialCommand>
{
    public async ValueTask ExecuteAsync(InitialCommand command, CancellationToken cancellationToken = default)
    {
        var month = timeUtnsil.GetDateOnly(command.Month, 480);

        var brandGameCredit = await brandGameCreditRepository.GetAsync(
            brandId: command.BrandId,
            month: month.AddMonths(-1),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var amount = 0.0m;

        if (brandGameCredit != null)
        {
            var credit = brandGameCredit.InitialAmount + brandGameCredit.CreditAmount - brandGameCredit.DebitAmount;
            amount = credit + brandGameCredit.ProfitAmount;
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

        await service.ExecuteAsync(
            command: new PublishCommand(
                Subject: BrandGameCreditsHandlerNatsConst.Credit,
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                Month: command.Month),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
