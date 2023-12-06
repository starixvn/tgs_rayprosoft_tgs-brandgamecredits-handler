using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Common.Protos;
using Tgs.BrandGameCredits.Handler.Core;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.BrandGameCredits.Handler.Messages;

public class CreditMessageHandler(
    ILogger<CreditMessageHandler> logger,
    IHandlerDispatcher dispatcher) : IMessageHandler
{
    public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var publish = BrandGameCreditPublish.Parser.ParseFrom(data.Span)
            ?? throw new NullReferenceException("The request data resolves to a null value");

        logger.LogInformation("LogOn:{logOn} Information publish:{publish}", DateTime.Now.ToString("o"), publish);

        return dispatcher.PushCommandAsync(
            command: new DispatcherCommand(
                ResellerId: publish.ResellerId.ToGuid(),
                ResellerCode: publish.ResellerCode,
                LicenseeId: publish.LicenseeId.ToGuid(),
                LicenseeCode: publish.LicenseeCode,
                BrandId: publish.BrandId.ToGuid(),
                BrandCode: publish.BrandCode,
                CurrentTime: publish.CurrentTime.ToDateTimeOffset(),
                DispatcherType: DispatcherType.Credit),
            cancellationToken: cancellationToken);
    }
}
