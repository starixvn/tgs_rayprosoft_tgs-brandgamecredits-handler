using System.Threading;
using NSubstitute;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Tgs.BrandGameCredits.Handler.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.UnitTest.ApplicationServices;

public class ProfitCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeBrandGameCreditTimeUtnsil = Substitute.For<IBrandGameCreditTimeUtnsil>();
        var fakeGameWagerStatDailyCloseRepository = Substitute.For<IGameWagerStatDailyCloseRepository>();
        var fakeBrandGameCreditRepository = Substitute.For<IBrandGameCreditRepository>();
        var fakePublishService = Substitute.For<ICommandService<PublishCommand>>();

        var command = new ProfitCommand(
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

        var gameWagerStats = new GameWagerStatDailyCloseInfo[]{
            new(
                ResellerId: command.ResellerId,
                LicenseeId: command.LicenseeId,
                BrandId: command.BrandId,
                GameProviderId: Guid.NewGuid(),
                DayStart: monthDateOnly,
                DayEnd: monthDateOnly.AddMonths(+1),
                Count: 1000,
                Bet: 1000,
                ValidBet: 1000,
                Settle: 500,
                Commission: 0,
                Turnover: 1000,
                WinLoss: -500),
            new(
                ResellerId: command.ResellerId,
                LicenseeId: command.LicenseeId,
                BrandId: command.BrandId,
                GameProviderId: Guid.NewGuid(),
                DayStart: monthDateOnly,
                DayEnd: monthDateOnly.AddMonths(+1),
                Count: 500,
                Bet: 500,
                ValidBet: 500,
                Settle: 750,
                Commission: 0,
                Turnover: 500,
                WinLoss: 250)
        };
        _ = fakeGameWagerStatDailyCloseRepository.QueryAsync(
            brandId: Arg.Is(command.BrandId),
            statOnStart: Arg.Is(monthTimeZone),
            statOnEnd: Arg.Is(monthTimeZone.AddMonths(1)),
            dayStart: Arg.Is(monthDateOnly),
            dayEnd: Arg.Is(monthDateOnly.AddMonths(1)),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameWagerStats.ToAsyncEnumerable());

        var service = new ProfitCommandService(
            fakeBrandGameCreditTimeUtnsil,
            fakeGameWagerStatDailyCloseRepository,
            fakeBrandGameCreditRepository,
            fakePublishService);

        await service.ExecuteAsync(command: command);

        _ = fakeBrandGameCreditRepository
            .Received(1)
            .UpdateProfitAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            amount: Arg.Is(gameWagerStats.Sum(selector => selector.WinLoss)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakePublishService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new PublishCommand(
                Subject: BrandGameCreditsHandlerNatsConst.Settlement,
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: monthTimeZone.AddMonths(-1))),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task No_Credit_This_Month()
    {
        var fakeBrandGameCreditTimeUtnsil = Substitute.For<IBrandGameCreditTimeUtnsil>();
        var fakeGameWagerStatDailyCloseRepository = Substitute.For<IGameWagerStatDailyCloseRepository>();
        var fakeBrandGameCreditRepository = Substitute.For<IBrandGameCreditRepository>();
        var fakePublishService = Substitute.For<ICommandService<PublishCommand>>();

        var command = new ProfitCommand(
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

        _ = fakeBrandGameCreditRepository.GetAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns((BrandGameCreditInfo?)null);

        var service = new ProfitCommandService(
            fakeBrandGameCreditTimeUtnsil,
            fakeGameWagerStatDailyCloseRepository,
            fakeBrandGameCreditRepository,
            fakePublishService);

        await service.ExecuteAsync(command: command);

        _ = fakeGameWagerStatDailyCloseRepository
            .Received(0)
            .QueryAsync(
            brandId: Arg.Is(command.BrandId),
            statOnStart: Arg.Is(monthTimeZone),
            statOnEnd: Arg.Is(monthTimeZone.AddMonths(1)),
            dayStart: Arg.Is(monthDateOnly),
            dayEnd: Arg.Is(monthDateOnly.AddMonths(1)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeBrandGameCreditRepository
            .Received(0)
            .UpdateProfitAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            amount: Arg.Any<decimal>(),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakePublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new PublishCommand(
                Subject: BrandGameCreditsHandlerNatsConst.Settlement,
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode,
                CurrentTime: monthTimeZone.AddMonths(-1))),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
