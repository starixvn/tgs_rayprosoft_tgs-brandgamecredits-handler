using Tgs.BrandGameCredits.Handler.Common.Protos;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.BrandGameCredits.Handler.Messages;

public class ProfitMessageHandler(
    ILogger<ProfitMessageHandler> logger,
    ICommandService<ProfitCommand> service) : IMessageHandler
{
    public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var publish = BrandGameCreditPublish.Parser.ParseFrom(data.Span)
            ?? throw new NullReferenceException("The request data resolves to a null value");

        logger.LogInformation("LogOn:{logOn} Information publish:{publish}", DateTime.Now.ToString("o"), publish);

        return service.ExecuteAsync(
            command: new ProfitCommand(
                ResellerId: publish.ResellerId.ToGuid(),
                ResellerCode: publish.ResellerCode,
                LicenseeId: publish.LicenseeId.ToGuid(),
                LicenseeCode: publish.LicenseeCode,
                BrandId: publish.BrandId.ToGuid(),
                BrandCode: publish.BrandCode,
                Month: publish.Month.ToDateTimeOffset()),
            cancellationToken: cancellationToken);
    }
}
