using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Tgs.BrandGameCredits.Handler.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.UnitTest.ApplicationServices;

public class SettlementCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<SettlementCommandService>.Instance;
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeBrandGameCreditTimeUtnsil = Substitute.For<IBrandGameCreditTimeUtnsil>();
        var fakeBrandGameCreditRepository = Substitute.For<IBrandGameCreditRepository>();
        var fakeCreditSerivce = Substitute.For<ICommandService<CreditCommand>>();
        var fakeDebitService = Substitute.For<ICommandService<DebitCommand>>();
        var fakeProfitService = Substitute.For<ICommandService<ProfitCommand>>();
        var fakePublishService = Substitute.For<ICommandService<PublishCommand>>();

        var command = new SettlementCommand(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            CurrentTime: new DateTimeOffset(2023, 11, 01, 00, 00, 00, TimeSpan.FromMinutes(480)));

        var monthTimeZone = new DateTimeOffset(2023, 11, 01, 00, 00, 00, TimeSpan.FromMinutes(480));
        _ = fakeBrandGameCreditTimeUtnsil.Get1stOfEveryMonthTimeZone(
            currentTime: Arg.Is(command.CurrentTime),
            timeSpanMinutes: Arg.Is(480))
            .Returns(monthTimeZone);

        var monthDateOnly = DateOnly.Parse("2023-11-01");
        _ = fakeBrandGameCreditTimeUtnsil.Get1stOfEveryMonthDateOnly(
            currentTime: Arg.Is(monthTimeZone),
            timeSpanMinutes: Arg.Is(480))
            .Returns(monthDateOnly);

        var currentTime = new DateTimeOffset(2023, 12, 06, 15, 30, 01, TimeSpan.FromMinutes(480));
        _ = fakeSystemClock.CurrentTime.Returns(currentTime);

        var brandGameCredit = new BrandGameCreditInfo(
            ResellerId: command.ResellerId,
            LicenseeId: command.LicenseeId,
            BrandId: command.BrandId,
            Month: monthDateOnly,
            InitialAmount: 100000,
            CreditAmount: 12345,
            DebitAmount: 2345,
            ProfitAmount: -99999
            );
        _ = fakeBrandGameCreditRepository.GetAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(brandGameCredit);

        var service = new SettlementCommandService(
            fakeLogger,
            fakeSystemClock,
            fakeBrandGameCreditTimeUtnsil,
            fakeBrandGameCreditRepository,
            fakeCreditSerivce,
            fakeDebitService,
            fakeProfitService,
            fakePublishService);

        await service.ExecuteAsync(command: command);

        _ = fakeCreditSerivce
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new CreditCommand(
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeProfitService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new ProfitCommand(
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeDebitService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new DebitCommand(
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakePublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new PublishCommand(
                Subject: BrandGameCreditsHandlerNatsConst.Create,
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: currentTime)),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task No_Credit_This_Month()
    {
        var fakeLogger = NullLogger<SettlementCommandService>.Instance;
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeBrandGameCreditTimeUtnsil = Substitute.For<IBrandGameCreditTimeUtnsil>();
        var fakeBrandGameCreditRepository = Substitute.For<IBrandGameCreditRepository>();
        var fakeCreditSerivce = Substitute.For<ICommandService<CreditCommand>>();
        var fakeDebitService = Substitute.For<ICommandService<DebitCommand>>();
        var fakeProfitService = Substitute.For<ICommandService<ProfitCommand>>();
        var fakePublishService = Substitute.For<ICommandService<PublishCommand>>();

        var command = new SettlementCommand(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            CurrentTime: new DateTimeOffset(2023, 11, 01, 00, 00, 00, TimeSpan.FromMinutes(480)));

        var monthTimeZone = new DateTimeOffset(2023, 11, 01, 00, 00, 00, TimeSpan.FromMinutes(480));
        _ = fakeBrandGameCreditTimeUtnsil.Get1stOfEveryMonthTimeZone(
            currentTime: Arg.Is(command.CurrentTime),
            timeSpanMinutes: Arg.Is(480))
            .Returns(monthTimeZone);

        var monthDateOnly = DateOnly.Parse("2023-11-01");
        _ = fakeBrandGameCreditTimeUtnsil.Get1stOfEveryMonthDateOnly(
            currentTime: Arg.Is(monthTimeZone),
            timeSpanMinutes: Arg.Is(480))
            .Returns(monthDateOnly);

        var currentTime = new DateTimeOffset(2023, 12, 06, 15, 30, 01, TimeSpan.FromMinutes(480));
        _ = fakeSystemClock.CurrentTime.Returns(currentTime);

        _ = fakeBrandGameCreditRepository.GetAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns((BrandGameCreditInfo?)null);

        var service = new SettlementCommandService(
            fakeLogger,
            fakeSystemClock,
            fakeBrandGameCreditTimeUtnsil,
            fakeBrandGameCreditRepository,
            fakeCreditSerivce,
            fakeDebitService,
            fakeProfitService,
            fakePublishService);

        await service.ExecuteAsync(command: command);

        _ = fakeCreditSerivce
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new CreditCommand(
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeProfitService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new ProfitCommand(
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeDebitService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new DebitCommand(
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakePublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new PublishCommand(
                Subject: BrandGameCreditsHandlerNatsConst.Create,
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: currentTime)),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
