using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Tgs.GameWagers.Stat.Daily.Common.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.BrandGameCredits.Handler.Messages;

public class StatCloseMessageHandler(
    ILogger<StatCloseMessageHandler> logger,
    ICommandService<StatCloseCommand> service) : IMessageHandler<GameWagersStatDailyFinishMessage>
{
    public ValueTask HandleAsync(string subject, GameWagersStatDailyFinishMessage data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This receiving message data info.({data})",
            DateTime.Now.ToString("o"),
            subject,
            data);

        return service.ExecuteAsync(
            command: new StatCloseCommand(
                ResellerId: data.ResellerId.ToGuid(),
                ResellerCode: data.ResellerCode,
                LicenseeId: data.LicenseeId.ToGuid(),
                LicenseeCode: data.LicenseeCode,
                BrandId: data.BrandId.ToGuid(),
                BrandCode: data.BrandCode),
            cancellationToken: cancellationToken);
    }
}
