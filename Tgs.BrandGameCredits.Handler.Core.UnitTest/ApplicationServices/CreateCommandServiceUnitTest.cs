using NSubstitute;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Tgs.BrandGameCredits.Handler.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.UnitTest.ApplicationServices;

public class CreateCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeBrandGameCreditTimeUtnsil = Substitute.For<IBrandGameCreditTimeUtnsil>();
        var fakeBrandGameCreditRepository = Substitute.For<IBrandGameCreditRepository>();
        var fakePublishService = Substitute.For<ICommandService<PublishCommand>>();

        var command = new CreateCommand(
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

        var service = new CreateCommandService(
            fakeBrandGameCreditTimeUtnsil,
            fakeBrandGameCreditRepository,
            fakePublishService);

        await service.ExecuteAsync(command: command);

        _ = fakeBrandGameCreditRepository
            .Received(1)
            .AddAsync(
            resellerId: Arg.Is(command.ResellerId),
            licenseeId: Arg.Is(command.LicenseeId),
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakePublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new PublishCommand(
                Subject: BrandGameCreditsHandlerNatsConst.Initial,
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
    public async Task Have_Credit_For_The_Month()
    {
        var fakeBrandGameCreditTimeUtnsil = Substitute.For<IBrandGameCreditTimeUtnsil>();
        var fakeBrandGameCreditRepository = Substitute.For<IBrandGameCreditRepository>();
        var fakePublishService = Substitute.For<ICommandService<PublishCommand>>();

        var command = new CreateCommand(
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
            ProfitAmount: -99999);
        _ = fakeBrandGameCreditRepository.GetAsync(
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(brandGameCredit);

        var service = new CreateCommandService(
            fakeBrandGameCreditTimeUtnsil,
            fakeBrandGameCreditRepository,
            fakePublishService);

        await service.ExecuteAsync(command: command);

        _ = fakeBrandGameCreditRepository
            .Received(0)
            .AddAsync(
            resellerId: Arg.Is(command.ResellerId),
            licenseeId: Arg.Is(command.LicenseeId),
            brandId: Arg.Is(command.BrandId),
            month: Arg.Is(monthDateOnly),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakePublishService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new PublishCommand(
                Subject: BrandGameCreditsHandlerNatsConst.Initial,
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
