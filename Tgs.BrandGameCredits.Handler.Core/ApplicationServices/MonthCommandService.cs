using Tgs.BrandGameCredits.Handler.Common;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class MonthCommandService(
    ISystemClock systemClock,
    IBrandGameCreditTimeUtnsil timeUtnsil,
    IBrandGameCreditRepository repository,
    ICommandService<PublishCommand> service) : ICommandService<MonthCommand>
{
    public async ValueTask ExecuteAsync(MonthCommand command, CancellationToken cancellationToken = default)
    {
        var currentTime = systemClock.CurrentTime;
        var month = timeUtnsil.GetTimeZone(currentTime, 480);

        if (await repository.GetAsync(
            command.BrandId,
            DateOnly.FromDateTime(month.Date),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false) is null)
        {
            await repository.AddAsync(
                resellerId: command.ResellerId,
                licenseeId: command.LicenseeId,
                brandId: command.BrandId,
                month: DateOnly.FromDateTime(month.Date),
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
                Month: month),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
