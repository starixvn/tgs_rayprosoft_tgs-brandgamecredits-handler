using NSubstitute;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Tgs.BrandGameCredits.Handler.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.UnitTest.ApplicationServices;

public class ProfitCommandServiceUnitTest
{
    [Fact]
    public async Task One_Of_The_Players_Of_This_Game_Provider_Has_A_Negative_Win_Or_Loss()
    {
        var fakeBrandGameCreditTimeUtnsil = Substitute.For<IBrandGameCreditTimeUtnsil>();
        var fakeGameWagerStatDailyCloseRepository = Substitute.For<IGameWagerStatDailyCloseRepository>();
        var fakeBrandGameCreditProfitRepository = Substitute.For<IBrandGameCreditProfitRepository>();
        var fakeBrandGameCreditRepository = Substitute.For<IBrandGameCreditRepository>();
        var fakePublishService = Substitute.For<ICommandService<PublishCommand>>();
        var fakeUpdatePublishService = Substitute.For<ICommandService<UpdatePublishCommand>>();

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
            fakeBrandGameCreditProfitRepository,
            fakeBrandGameCreditRepository,
            fakePublishService,
            fakeUpdatePublishService);

        await service.ExecuteAsync(command: command);

        _ = fakeBrandGameCreditProfitRepository
            .Received(1)
            .DeleteBatchAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            cancellationToken: Arg.Any<CancellationToken>());

        var gameCreditProfitAdds = gameWagerStats.GroupBy(
            keySelector: keySelector => new
            {
                keySelector.ResellerId,
                keySelector.LicenseeId,
                keySelector.BrandId,
                keySelector.GameProviderId
            })
            .Select(selector => new BrandGameCreditProfitAdd(
                BrandId: selector.Key.BrandId,
                Month: monthDateOnly,
                GameProviderId: selector.Key.GameProviderId,
                Count: selector.Sum(selector => selector.Count),
                Bet: selector.Sum(selector => selector.Bet),
                ValidBet: selector.Sum(selector => selector.ValidBet),
                Settle: selector.Sum(selector => selector.Settle),
                Commission: selector.Sum(selector => selector.Commission),
                Turnover: selector.Sum(selector => selector.Turnover),
                WinLoss: selector.Sum(selector => selector.WinLoss)))
            .ToArray()
            ?? Enumerable.Empty<BrandGameCreditProfitAdd>();
        _ = fakeBrandGameCreditProfitRepository
            .Received(1)
            .InsertBatchAsync(
            source: Arg.Any<IEnumerable<BrandGameCreditProfitAdd>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeBrandGameCreditRepository
            .Received(1)
            .UpdateProfitAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            amount: Arg.Is(500m),
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

        _ = fakeUpdatePublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new UpdatePublishCommand(
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode)),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Players_From_This_Game_Provider_Win()
    {
        var fakeBrandGameCreditTimeUtnsil = Substitute.For<IBrandGameCreditTimeUtnsil>();
        var fakeGameWagerStatDailyCloseRepository = Substitute.For<IGameWagerStatDailyCloseRepository>();
        var fakeBrandGameCreditProfitRepository = Substitute.For<IBrandGameCreditProfitRepository>();
        var fakeBrandGameCreditRepository = Substitute.For<IBrandGameCreditRepository>();
        var fakePublishService = Substitute.For<ICommandService<PublishCommand>>();
        var fakeUpdatePublishService = Substitute.For<ICommandService<UpdatePublishCommand>>();

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
                Settle: 1500,
                Commission: 0,
                Turnover: 1000,
                WinLoss: 500),
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
            fakeBrandGameCreditProfitRepository,
            fakeBrandGameCreditRepository,
            fakePublishService,
            fakeUpdatePublishService);

        await service.ExecuteAsync(command: command);

        _ = fakeBrandGameCreditProfitRepository
            .Received(1)
            .DeleteBatchAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            cancellationToken: Arg.Any<CancellationToken>());

        var gameCreditProfitAdds = gameWagerStats.GroupBy(
            keySelector: keySelector => new
            {
                keySelector.ResellerId,
                keySelector.LicenseeId,
                keySelector.BrandId,
                keySelector.GameProviderId
            })
            .Select(selector => new BrandGameCreditProfitAdd(
                BrandId: selector.Key.BrandId,
                Month: monthDateOnly,
                GameProviderId: selector.Key.GameProviderId,
                Count: selector.Sum(selector => selector.Count),
                Bet: selector.Sum(selector => selector.Bet),
                ValidBet: selector.Sum(selector => selector.ValidBet),
                Settle: selector.Sum(selector => selector.Settle),
                Commission: selector.Sum(selector => selector.Commission),
                Turnover: selector.Sum(selector => selector.Turnover),
                WinLoss: selector.Sum(selector => selector.WinLoss)))
            .ToArray()
            ?? Enumerable.Empty<BrandGameCreditProfitAdd>();
        _ = fakeBrandGameCreditProfitRepository
            .Received(1)
            .InsertBatchAsync(
            source: Arg.Any<IEnumerable<BrandGameCreditProfitAdd>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeBrandGameCreditRepository
            .Received(1)
            .UpdateProfitAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            amount: Arg.Is(0.0m),
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

        _ = fakeUpdatePublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new UpdatePublishCommand(
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode)),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task No_Credit_This_Month()
    {
        var fakeBrandGameCreditTimeUtnsil = Substitute.For<IBrandGameCreditTimeUtnsil>();
        var fakeGameWagerStatDailyCloseRepository = Substitute.For<IGameWagerStatDailyCloseRepository>();
        var fakeBrandGameCreditProfitRepository = Substitute.For<IBrandGameCreditProfitRepository>();
        var fakeBrandGameCreditRepository = Substitute.For<IBrandGameCreditRepository>();
        var fakePublishService = Substitute.For<ICommandService<PublishCommand>>();
        var fakeUpdatePublishService = Substitute.For<ICommandService<UpdatePublishCommand>>();

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
            fakeBrandGameCreditProfitRepository,
            fakeBrandGameCreditRepository,
            fakePublishService,
            fakeUpdatePublishService);

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

        _ = fakeBrandGameCreditProfitRepository
            .Received(0)
            .DeleteBatchAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeBrandGameCreditProfitRepository
            .Received(0)
            .InsertBatchAsync(
            source: Arg.Any<IEnumerable<BrandGameCreditProfitAdd>>(),
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

        _ = fakeUpdatePublishService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new UpdatePublishCommand(
                ResellerId: command.ResellerId,
                ResellerCode: command.ResellerCode,
                LicenseeId: command.LicenseeId,
                LicenseeCode: command.LicenseeCode,
                BrandId: command.BrandId,
                BrandCode: command.BrandCode)),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
