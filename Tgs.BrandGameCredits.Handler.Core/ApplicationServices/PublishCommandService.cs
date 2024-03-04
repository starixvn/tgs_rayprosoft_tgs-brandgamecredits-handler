using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Tgs.BrandGameCredits.Handler.Common.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class PublishCommandService(
    ILogger<PublishCommandService> logger,
    IMessageSender messageSender) : ICommandService<PublishCommand>
{
    public ValueTask ExecuteAsync(PublishCommand command, CancellationToken cancellationToken = default)
    {
        var publish = new BrandGameCreditPublish
        {
            ResellerId = command.ResellerId,
            ResellerCode = command.ResellerCode,
            LicenseeId = command.LicenseeId,
            LicenseeCode = command.LicenseeCode,
            BrandId = command.BrandId,
            BrandCode = command.BrandCode,
            CurrentTime = command.CurrentTime.ToTimestamp()
        };

        logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:The content of this publish.({publish})",
            DateTime.Now.ToString("u"),
            command.Subject,
            publish);

        return messageSender.PublishAsync(
            subject: command.Subject,
            data: publish,
            cancellationToken: cancellationToken);
    }
}
