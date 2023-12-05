using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class DebitCommandService(
    IBrandGameCreditTimeUtnsil timeUtnsil,
    IBrandGameCreditRepository brandGameCreditRepository,
    ICommandService<PublishCommand> service) : ICommandService<DebitCommand>
{
    public async ValueTask ExecuteAsync(DebitCommand command, CancellationToken cancellationToken = default)
    {
        var month = timeUtnsil.GetDateOnly(command.Month, 480);

        await brandGameCreditRepository.UpdateDebitAsync(
            brandId: command.BrandId,
            month: month,
            amount: 0.0m,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await service.ExecuteAsync(
            command: new PublishCommand(
                Subject: BrandGameCreditsHandlerNatsConst.Profit,
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
