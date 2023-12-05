using Tgs.BrandGameCredits.Handler.Common;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class CreditCommandService(
    IBrandGameCreditTimeUtnsil timeUtnsil,
    IBrandGameCreditRepository brandGameCreditRepository,
    ICommandService<PublishCommand> service) : ICommandService<CreditCommand>
{
    public async ValueTask ExecuteAsync(CreditCommand command, CancellationToken cancellationToken = default)
    {
        var month = timeUtnsil.GetDateOnly(command.Month, 480);

        await brandGameCreditRepository.UpdateCreditAsync(
            brandId: command.BrandId,
            month: month,
            amount: 0.0m,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await service.ExecuteAsync(
            command: new PublishCommand(
                Subject: BrandGameCreditsHandlerNatsConst.Debit,
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
