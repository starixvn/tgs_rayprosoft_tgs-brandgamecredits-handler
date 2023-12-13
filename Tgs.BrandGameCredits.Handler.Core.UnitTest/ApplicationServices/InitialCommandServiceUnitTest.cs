using NSubstitute;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Common.Models;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Tgs.BrandGameCredits.Handler.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.UnitTest.ApplicationServices;

public class InitialCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeBrandGameCreditTimeUtnsil = Substitute.For<IBrandGameCreditTimeUtnsil>();
        var fakeBrandGameCreditComputingUtnsil = Substitute.For<IBrandGameCreditComputingUtnsil>();
        var fakeBrandGameCreditRepository = Substitute.For<IBrandGameCreditRepository>();
        var fakeInitialPublishService = Substitute.For<ICommandService<InitialPublishCommand>>();

        var command = new InitialCommand(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            CurrentTime: new DateTimeOffset(2023, 12, 06, 10, 55, 26, TimeSpan.FromMinutes(480)));

        var monthTimeZone = new DateTimeOffset(2023, 12, 01, 00, 00, 00, TimeSpan.FromMinutes(480));
        _ = fakeBrandGameCreditTimeUtnsil.Get1stOfEveryMonthTimeZone(
            currentTime: Arg.Is(command.CurrentTime),
            timeSpanMinutes: Arg.Is(480))
            .Returns(monthTimeZone);

        var monthDateOnly = DateOnly.Parse("2023-12-01");
        _ = fakeBrandGameCreditTimeUtnsil.Get1stOfEveryMonthDateOnly(
            currentTime: Arg.Is(monthTimeZone),
            timeSpanMinutes: Arg.Is(480))
            .Returns(monthDateOnly);

        var lastMonthBrandGameCredit = new BrandGameCreditInfo(
            ResellerId: command.ResellerId,
            LicenseeId: command.LicenseeId,
            BrandId: command.BrandId,
            Month: monthDateOnly.AddMonths(-1),
            InitialAmount: 100000,
            CreditAmount: 12345,
            DebitAmount: 2345,
            ProfitAmount: -99999
            );
        _ = fakeBrandGameCreditRepository.GetAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly.AddMonths(-1)),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(lastMonthBrandGameCredit);

        var creditCalculationInfo = new CreditCalculationInfo(
            110000m,
            99999m,
            90.9082m,
            10001m,
            9.0918m);
        _ = fakeBrandGameCreditComputingUtnsil.CreditCalculation(
            Arg.Is(lastMonthBrandGameCredit.InitialAmount),
            Arg.Is(lastMonthBrandGameCredit.CreditAmount),
            Arg.Is(lastMonthBrandGameCredit.DebitAmount),
            Arg.Is(lastMonthBrandGameCredit.ProfitAmount))
            .Returns(creditCalculationInfo);

        var service = new InitialCommandService(
            fakeBrandGameCreditTimeUtnsil,
            fakeBrandGameCreditComputingUtnsil,
            fakeBrandGameCreditRepository,
            fakeInitialPublishService);

        await service.ExecuteAsync(command: command);

        _ = fakeBrandGameCreditRepository
            .Received(1)
            .UpdateInitialAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            amount: Arg.Is(10001m),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeInitialPublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new InitialPublishCommand(
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Profit_Exceeds_Credit()
    {
        var fakeBrandGameCreditTimeUtnsil = Substitute.For<IBrandGameCreditTimeUtnsil>();
        var fakeBrandGameCreditComputingUtnsil = Substitute.For<IBrandGameCreditComputingUtnsil>();
        var fakeBrandGameCreditRepository = Substitute.For<IBrandGameCreditRepository>();
        var fakeInitialPublishService = Substitute.For<ICommandService<InitialPublishCommand>>();

        var command = new InitialCommand(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            CurrentTime: new DateTimeOffset(2023, 12, 06, 10, 55, 26, TimeSpan.FromMinutes(480)));

        var monthTimeZone = new DateTimeOffset(2023, 12, 01, 00, 00, 00, TimeSpan.FromMinutes(480));
        _ = fakeBrandGameCreditTimeUtnsil.Get1stOfEveryMonthTimeZone(
            currentTime: Arg.Is(command.CurrentTime),
            timeSpanMinutes: Arg.Is(480))
            .Returns(monthTimeZone);

        var monthDateOnly = DateOnly.Parse("2023-12-01");
        _ = fakeBrandGameCreditTimeUtnsil.Get1stOfEveryMonthDateOnly(
            currentTime: Arg.Is(monthTimeZone),
            timeSpanMinutes: Arg.Is(480))
            .Returns(monthDateOnly);

        var lastMonthBrandGameCredit = new BrandGameCreditInfo(
            ResellerId: command.ResellerId,
            LicenseeId: command.LicenseeId,
            BrandId: command.BrandId,
            Month: monthDateOnly.AddMonths(-1),
            InitialAmount: 100000,
            CreditAmount: 12345,
            DebitAmount: 2345,
            ProfitAmount: 1
            );
        _ = fakeBrandGameCreditRepository.GetAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly.AddMonths(-1)),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(lastMonthBrandGameCredit);

        var creditCalculationInfo = new CreditCalculationInfo(
            110000m,
            0,
            0,
            110000m,
            100);
        _ = fakeBrandGameCreditComputingUtnsil.CreditCalculation(
            Arg.Is(lastMonthBrandGameCredit.InitialAmount),
            Arg.Is(lastMonthBrandGameCredit.CreditAmount),
            Arg.Is(lastMonthBrandGameCredit.DebitAmount),
            Arg.Is(lastMonthBrandGameCredit.ProfitAmount))
            .Returns(creditCalculationInfo);

        var service = new InitialCommandService(
            fakeBrandGameCreditTimeUtnsil,
            fakeBrandGameCreditComputingUtnsil,
            fakeBrandGameCreditRepository,
            fakeInitialPublishService);

        await service.ExecuteAsync(command: command);

        _ = fakeBrandGameCreditRepository
            .Received(1)
            .UpdateInitialAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            amount: Arg.Is(110000m),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeInitialPublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new InitialPublishCommand(
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Insufficient_Credit()
    {
        var fakeBrandGameCreditTimeUtnsil = Substitute.For<IBrandGameCreditTimeUtnsil>();
        var fakeBrandGameCreditComputingUtnsil = Substitute.For<IBrandGameCreditComputingUtnsil>();
        var fakeBrandGameCreditRepository = Substitute.For<IBrandGameCreditRepository>();
        var fakeInitialPublishService = Substitute.For<ICommandService<InitialPublishCommand>>();

        var command = new InitialCommand(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            CurrentTime: new DateTimeOffset(2023, 12, 06, 10, 55, 26, TimeSpan.FromMinutes(480)));

        var monthTimeZone = new DateTimeOffset(2023, 12, 01, 00, 00, 00, TimeSpan.FromMinutes(480));
        _ = fakeBrandGameCreditTimeUtnsil.Get1stOfEveryMonthTimeZone(
            currentTime: Arg.Is(command.CurrentTime),
            timeSpanMinutes: Arg.Is(480))
            .Returns(monthTimeZone);

        var monthDateOnly = DateOnly.Parse("2023-12-01");
        _ = fakeBrandGameCreditTimeUtnsil.Get1stOfEveryMonthDateOnly(
            currentTime: Arg.Is(monthTimeZone),
            timeSpanMinutes: Arg.Is(480))
            .Returns(monthDateOnly);

        var lastMonthBrandGameCredit = new BrandGameCreditInfo(
            ResellerId: command.ResellerId,
            LicenseeId: command.LicenseeId,
            BrandId: command.BrandId,
            Month: monthDateOnly.AddMonths(-1),
            InitialAmount: 100000,
            CreditAmount: 12345,
            DebitAmount: 2345,
            ProfitAmount: -110001
            );
        _ = fakeBrandGameCreditRepository.GetAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly.AddMonths(-1)),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(lastMonthBrandGameCredit);

        var creditCalculationInfo = new CreditCalculationInfo(
            -1,
            -1,
            100,
            0,
            0);
        _ = fakeBrandGameCreditComputingUtnsil.CreditCalculation(
            Arg.Is(lastMonthBrandGameCredit.InitialAmount),
            Arg.Is(lastMonthBrandGameCredit.CreditAmount),
            Arg.Is(lastMonthBrandGameCredit.DebitAmount),
            Arg.Is(lastMonthBrandGameCredit.ProfitAmount))
            .Returns(creditCalculationInfo);

        var service = new InitialCommandService(
            fakeBrandGameCreditTimeUtnsil,
            fakeBrandGameCreditComputingUtnsil,
            fakeBrandGameCreditRepository,
            fakeInitialPublishService);

        await service.ExecuteAsync(command: command);

        _ = fakeBrandGameCreditRepository
            .Received(1)
            .UpdateInitialAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            amount: Arg.Is(-1m),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeInitialPublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new InitialPublishCommand(
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
