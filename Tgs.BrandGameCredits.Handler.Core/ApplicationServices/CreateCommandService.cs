using Tgs.BrandGameCredits.Handler.Common;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class CreateCommandService(
    IBrandGameCreditTimeUtnsil timeUtnsil,
    IBrandGameCreditRepository brandGameCreditRepository,
    ICommandService<PublishCommand> service) : ICommandService<CreateCommand>
{
    public async ValueTask ExecuteAsync(CreateCommand command, CancellationToken cancellationToken = default)
    {
        var monthTimeZone = timeUtnsil.Get1stOfEveryMonthTimeZone(command.CurrentTime, 480);
        var monthDateOnly = timeUtnsil.Get1stOfEveryMonthDateOnly(monthTimeZone, 480);

        var brandGameCredit = await brandGameCreditRepository.GetAsync(
            brandId: command.BrandId,
            month: monthDateOnly,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (brandGameCredit is null)
        {
            await brandGameCreditRepository.AddAsync(
                resellerId: command.ResellerId,
                licenseeId: command.LicenseeId,
                brandId: command.BrandId,
                month: monthDateOnly,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        await service.ExecuteAsync(
            command: new PublishCommand(
                Subject: BrandGameCreditsHandlerNatsConst.Initial,
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
