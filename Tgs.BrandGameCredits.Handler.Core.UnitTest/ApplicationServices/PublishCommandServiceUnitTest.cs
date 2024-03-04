using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Common.Protos;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.BrandGameCredits.Handler.Core.UnitTest.ApplicationServices;

public class PublishCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<PublishCommandService>.Instance;
        var fakeMessageSender = Substitute.For<IMessageSender>();

        var command = new PublishCommand(
            Subject: BrandGameCreditsHandlerNatsConst.Profit,
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            CurrentTime: new DateTimeOffset(2023, 11, 01, 00, 00, 00, TimeSpan.FromMinutes(480)));

        var service = new PublishCommandService(
            fakeLogger,
            fakeMessageSender);

        await service.ExecuteAsync(command: command);

        _ = fakeMessageSender
            .Received(1)
            .PublishAsync(
            subject: Arg.Is(command.Subject),
            data: Arg.Is(new BrandGameCreditPublish
            {
                ResellerId = command.ResellerId,
                ResellerCode = command.ResellerCode,
                LicenseeId = command.LicenseeId,
                LicenseeCode = command.LicenseeCode,
                BrandId = command.BrandId,
                BrandCode = command.BrandCode,
                CurrentTime = command.CurrentTime.ToTimestamp()
            }),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
