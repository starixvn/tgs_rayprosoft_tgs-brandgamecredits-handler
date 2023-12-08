using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class SettlementCommandService(
    ILogger<SettlementCommandService> logger,
    ISystemClock systemClock,
    IBrandGameCreditTimeUtnsil timeUtnsil,
    IBrandGameCreditRepository brandGameCreditRepository,
    ICommandService<CreditCommand> creditService,
    ICommandService<DebitCommand> debitService,
    ICommandService<ProfitCommand> profitService,
    ICommandService<PublishCommand> service) : ICommandService<SettlementCommand>
{
    public async ValueTask ExecuteAsync(SettlementCommand command, CancellationToken cancellationToken = default)
    {
        var monthTimeZone = timeUtnsil.Get1stOfEveryMonthTimeZone(command.CurrentTime, 480);
        var monthDateOnly = timeUtnsil.Get1stOfEveryMonthDateOnly(monthTimeZone, 480);

        logger.LogWarning("LogOn:{logOn} Message:This is used for monthly({month}) settlement of the brand({brand}).",
            DateTime.Now.ToString("o"),
            monthDateOnly,
            command.BrandCode);

        var brandGameCredit = await brandGameCreditRepository.GetAsync(
            brandId: command.BrandId,
            month: monthDateOnly,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (brandGameCredit is not null)
        {
            await creditService.ExecuteAsync(
                    command: new CreditCommand(
                        ResellerId: command.ResellerId,
                        ResellerCode: command.ResellerCode,
                        LicenseeId: command.LicenseeId,
                        LicenseeCode: command.LicenseeCode,
                        BrandId: command.BrandId,
                        BrandCode: command.BrandCode,
                        CurrentTime: command.CurrentTime),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            await debitService.ExecuteAsync(
                    command: new DebitCommand(
                        ResellerId: command.ResellerId,
                        ResellerCode: command.ResellerCode,
                        LicenseeId: command.LicenseeId,
                        LicenseeCode: command.LicenseeCode,
                        BrandId: command.BrandId,
                        BrandCode: command.BrandCode,
                        CurrentTime: command.CurrentTime),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            await profitService.ExecuteAsync(
                    command: new ProfitCommand(
                        ResellerId: command.ResellerId,
                        ResellerCode: command.ResellerCode,
                        LicenseeId: command.LicenseeId,
                        LicenseeCode: command.LicenseeCode,
                        BrandId: command.BrandId,
                        BrandCode: command.BrandCode,
                        CurrentTime: command.CurrentTime),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            /*
             var tasks = new Task[] {
                Task.Run(async () => {await creditService.ExecuteAsync(
                    command: new CreditCommand(
                        ResellerId: command.ResellerId,
                        ResellerCode: command.ResellerCode,
                        LicenseeId: command.LicenseeId,
                        LicenseeCode: command.LicenseeCode,
                        BrandId: command.BrandId,
                        BrandCode: command.BrandCode,
                        CurrentTime: command.CurrentTime),
                    cancellationToken:cancellationToken)
                    .ConfigureAwait (false); },
                    cancellationToken:cancellationToken),
                Task.Run(async () => {await debitService.ExecuteAsync(
                    command: new DebitCommand(
                        ResellerId: command.ResellerId,
                        ResellerCode: command.ResellerCode,
                        LicenseeId: command.LicenseeId,
                        LicenseeCode: command.LicenseeCode,
                        BrandId: command.BrandId,
                        BrandCode: command.BrandCode,
                        CurrentTime: command.CurrentTime),
                    cancellationToken:cancellationToken)
                    .ConfigureAwait (false); },
                    cancellationToken:cancellationToken),
                Task.Run(async () => {await profitService.ExecuteAsync(
                    command: new ProfitCommand(
                        ResellerId: command.ResellerId,
                        ResellerCode: command.ResellerCode,
                        LicenseeId: command.LicenseeId,
                        LicenseeCode: command.LicenseeCode,
                        BrandId: command.BrandId,
                        BrandCode: command.BrandCode,
                        CurrentTime: command.CurrentTime),
                    cancellationToken:cancellationToken)
                    .ConfigureAwait (false); },
                    cancellationToken:cancellationToken)
            };
            await Task.WhenAll(tasks).ConfigureAwait(false);
            */
        }

        var currentTime = systemClock.CurrentTime;

        await service.ExecuteAsync(
            command: new PublishCommand(
                Subject: BrandGameCreditsHandlerNatsConst.Create,
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: currentTime),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
