using NSubstitute;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.UnitTest.ApplicationServices;

public class DispatcherCommandServiceUnitTest
{
    [Fact]
    public async Task Create()
    {
        var fakeCreateService = Substitute.For<ICommandService<CreateCommand>>();
        var fakeInitialService = Substitute.For<ICommandService<InitialCommand>>();
        var fakeCreditService = Substitute.For<ICommandService<CreditCommand>>();
        var fakeDebitService = Substitute.For<ICommandService<DebitCommand>>();
        var fakeProfitService = Substitute.For<ICommandService<ProfitCommand>>();
        var fakeSettlementService = Substitute.For<ICommandService<SettlementCommand>>();

        var command = new DispatcherCommand(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            CurrentTime: new DateTimeOffset(2023, 12, 06, 10, 55, 26, TimeSpan.FromMinutes(480)),
            DispatcherType.Create);

        var service = new DispatcherCommandService(
            fakeCreateService,
            fakeInitialService,
            fakeCreditService,
            fakeDebitService,
            fakeProfitService,
            fakeSettlementService);

        await service.ExecuteAsync(command: command);

        _ = fakeCreateService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new CreateCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeInitialService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new InitialCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeCreditService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new CreditCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeDebitService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new DebitCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeProfitService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new ProfitCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeSettlementService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new SettlementCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Initial()
    {
        var fakeCreateService = Substitute.For<ICommandService<CreateCommand>>();
        var fakeInitialService = Substitute.For<ICommandService<InitialCommand>>();
        var fakeCreditService = Substitute.For<ICommandService<CreditCommand>>();
        var fakeDebitService = Substitute.For<ICommandService<DebitCommand>>();
        var fakeProfitService = Substitute.For<ICommandService<ProfitCommand>>();
        var fakeSettlementService = Substitute.For<ICommandService<SettlementCommand>>();

        var command = new DispatcherCommand(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            CurrentTime: new DateTimeOffset(2023, 12, 06, 10, 55, 26, TimeSpan.FromMinutes(480)),
            DispatcherType.Initial);

        var service = new DispatcherCommandService(
            fakeCreateService,
            fakeInitialService,
            fakeCreditService,
            fakeDebitService,
            fakeProfitService,
            fakeSettlementService);

        await service.ExecuteAsync(command: command);

        _ = fakeCreateService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new CreateCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeInitialService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new InitialCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeCreditService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new CreditCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeDebitService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new DebitCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeProfitService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new ProfitCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeSettlementService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new SettlementCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Credit()
    {
        var fakeCreateService = Substitute.For<ICommandService<CreateCommand>>();
        var fakeInitialService = Substitute.For<ICommandService<InitialCommand>>();
        var fakeCreditService = Substitute.For<ICommandService<CreditCommand>>();
        var fakeDebitService = Substitute.For<ICommandService<DebitCommand>>();
        var fakeProfitService = Substitute.For<ICommandService<ProfitCommand>>();
        var fakeSettlementService = Substitute.For<ICommandService<SettlementCommand>>();

        var command = new DispatcherCommand(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            CurrentTime: new DateTimeOffset(2023, 12, 06, 10, 55, 26, TimeSpan.FromMinutes(480)),
            DispatcherType.Credit);

        var service = new DispatcherCommandService(
            fakeCreateService,
            fakeInitialService,
            fakeCreditService,
            fakeDebitService,
            fakeProfitService,
            fakeSettlementService);

        await service.ExecuteAsync(command: command);

        _ = fakeCreateService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new CreateCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeInitialService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new InitialCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeCreditService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new CreditCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeDebitService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new DebitCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeProfitService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new ProfitCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeSettlementService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new SettlementCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Debit()
    {
        var fakeCreateService = Substitute.For<ICommandService<CreateCommand>>();
        var fakeInitialService = Substitute.For<ICommandService<InitialCommand>>();
        var fakeCreditService = Substitute.For<ICommandService<CreditCommand>>();
        var fakeDebitService = Substitute.For<ICommandService<DebitCommand>>();
        var fakeProfitService = Substitute.For<ICommandService<ProfitCommand>>();
        var fakeSettlementService = Substitute.For<ICommandService<SettlementCommand>>();

        var command = new DispatcherCommand(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            CurrentTime: new DateTimeOffset(2023, 12, 06, 10, 55, 26, TimeSpan.FromMinutes(480)),
            DispatcherType.Debit);

        var service = new DispatcherCommandService(
            fakeCreateService,
            fakeInitialService,
            fakeCreditService,
            fakeDebitService,
            fakeProfitService,
            fakeSettlementService);

        await service.ExecuteAsync(command: command);

        _ = fakeCreateService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new CreateCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeInitialService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new InitialCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeCreditService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new CreditCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeDebitService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new DebitCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeProfitService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new ProfitCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeSettlementService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new SettlementCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Profit()
    {
        var fakeCreateService = Substitute.For<ICommandService<CreateCommand>>();
        var fakeInitialService = Substitute.For<ICommandService<InitialCommand>>();
        var fakeCreditService = Substitute.For<ICommandService<CreditCommand>>();
        var fakeDebitService = Substitute.For<ICommandService<DebitCommand>>();
        var fakeProfitService = Substitute.For<ICommandService<ProfitCommand>>();
        var fakeSettlementService = Substitute.For<ICommandService<SettlementCommand>>();

        var command = new DispatcherCommand(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            CurrentTime: new DateTimeOffset(2023, 12, 06, 10, 55, 26, TimeSpan.FromMinutes(480)),
            DispatcherType.Profit);

        var service = new DispatcherCommandService(
            fakeCreateService,
            fakeInitialService,
            fakeCreditService,
            fakeDebitService,
            fakeProfitService,
            fakeSettlementService);

        await service.ExecuteAsync(command: command);

        _ = fakeCreateService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new CreateCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeInitialService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new InitialCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeCreditService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new CreditCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeDebitService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new DebitCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeProfitService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new ProfitCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeSettlementService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new SettlementCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Settlement()
    {
        var fakeCreateService = Substitute.For<ICommandService<CreateCommand>>();
        var fakeInitialService = Substitute.For<ICommandService<InitialCommand>>();
        var fakeCreditService = Substitute.For<ICommandService<CreditCommand>>();
        var fakeDebitService = Substitute.For<ICommandService<DebitCommand>>();
        var fakeProfitService = Substitute.For<ICommandService<ProfitCommand>>();
        var fakeSettlementService = Substitute.For<ICommandService<SettlementCommand>>();

        var command = new DispatcherCommand(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            CurrentTime: new DateTimeOffset(2023, 12, 06, 10, 55, 26, TimeSpan.FromMinutes(480)),
            DispatcherType.Settlement);

        var service = new DispatcherCommandService(
            fakeCreateService,
            fakeInitialService,
            fakeCreditService,
            fakeDebitService,
            fakeProfitService,
            fakeSettlementService);

        await service.ExecuteAsync(command: command);

        _ = fakeCreateService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new CreateCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeInitialService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new InitialCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeCreditService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new CreditCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeDebitService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new DebitCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeProfitService
            .Received(0)
            .ExecuteAsync(
            command: Arg.Is(new ProfitCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeSettlementService
            .Received(1)
            .ExecuteAsync(
            command: Arg.Is(new SettlementCommand(
                command.ResellerId,
                command.ResellerCode,
                command.LicenseeId,
                command.LicenseeCode,
                command.BrandId,
                command.BrandCode,
                command.CurrentTime)),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
