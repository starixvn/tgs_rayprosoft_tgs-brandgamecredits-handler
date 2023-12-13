using NATS.Client.Service;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class DebitCommandService(
    IBrandGameCreditTimeUtnsil timeUtnsil,
    IBrandGameCreditTxnRepository brandGameCreditTxnRepository,
    IBrandGameCreditRepository brandGameCreditRepository,
    ICommandService<PublishCommand> publishService,
    ICommandService<UpdatePublishCommand> updatePublishService) : ICommandService<DebitCommand>
{
    public async ValueTask ExecuteAsync(DebitCommand command, CancellationToken cancellationToken = default)
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

        var amount = await brandGameCreditTxnRepository.SumDebitAsync(
            brandId: command.BrandId,
            createdOnStart: monthTimeZone,
            createdOnEnd: monthTimeZone.AddMonths(1),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await brandGameCreditRepository.UpdateDebitAsync(
            brandId: command.BrandId,
            month: monthDateOnly,
            amount: amount,
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
