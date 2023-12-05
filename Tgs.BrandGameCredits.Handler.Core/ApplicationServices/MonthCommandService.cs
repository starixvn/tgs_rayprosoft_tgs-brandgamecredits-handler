using Tgs.BrandGameCredits.Handler.Common;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class MonthCommandService(
    ISystemClock systemClock,
    IBrandGameCreditTimeUtnsil timeUtnsil,
    IBrandGameCreditRepository repository,
    ICommandService<MonthlySettlementCommand> monthlySettlementService,
    ICommandService<PublishCommand> service) : ICommandService<MonthCommand>
{
    public async ValueTask ExecuteAsync(MonthCommand command, CancellationToken cancellationToken = default)
    {
        var currentTime = systemClock.CurrentTime;
        var monthTimeZone = timeUtnsil.GetTimeZone(currentTime, 480);
        var month = timeUtnsil.GetDateOnly(monthTimeZone, 480);

        if (await repository.GetAsync(
            brandId: command.BrandId,
            month: month,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false) is null)
        {
            await monthlySettlementService.ExecuteAsync(
                command: new MonthlySettlementCommand(
                 ResellerId: command.ResellerId,
                 ResellerCode: command.ResellerCode,
                 LicenseeId: command.LicenseeId,
                 LicenseeCode: command.LicenseeCode,
                 BrandId: command.BrandId,
                 BrandCode: command.BrandCode,
                 Month: monthTimeZone.AddMonths(-1)),
                cancellationToken: cancellationToken)
                 .ConfigureAwait(false);
            await repository.AddAsync(
                resellerId: command.ResellerId,
                licenseeId: command.LicenseeId,
                brandId: command.BrandId,
                month: month,
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
                Month: monthTimeZone),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
