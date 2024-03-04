using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Common.Protos;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.BrandGameCredits.Handler.Core.UnitTest.ApplicationServices;

public class UpdatePublishCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<UpdatePublishCommandService>.Instance;
        var fakeMessageSender = Substitute.For<IMessageSender>();

        var command = new UpdatePublishCommand(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI");

        var service = new UpdatePublishCommandService(
            fakeLogger,
            fakeMessageSender);

        await service.ExecuteAsync(command: command);

        _ = fakeMessageSender
            .Received(1)
            .PublishAsync(
            subject: Arg.Is(BrandGameCreditsHandlerNatsConst.Update),
            data: Arg.Is(new BrandGameCreditUpdatePublish
            {
                ResellerId = command.ResellerId,
                ResellerCode = command.ResellerCode,
                LicenseeId = command.LicenseeId,
                LicenseeCode = command.LicenseeCode,
                BrandId = command.BrandId,
                BrandCode = command.BrandCode
            }),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
