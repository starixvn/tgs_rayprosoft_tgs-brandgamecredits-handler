using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Common.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class UpdatePublishCommandService(
    ILogger<UpdatePublishCommandService> logger,
    IMessageSender messageSender) : ICommandService<UpdatePublishCommand>
{
    public ValueTask ExecuteAsync(UpdatePublishCommand command, CancellationToken cancellationToken = default)
    {
        var publish = new BrandGameCreditUpdatePublish
        {
            ResellerId = command.ResellerId,
            ResellerCode = command.ResellerCode,
            LicenseeId = command.LicenseeId,
            LicenseeCode = command.LicenseeCode,
            BrandId = command.BrandId,
            BrandCode = command.BrandCode
        };

        logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:The content of this publish.({publish})",
            DateTime.Now.ToString("u"),
            BrandGameCreditsHandlerNatsConst.Update,
            publish);

        return messageSender.PublishAsync(
            subject: BrandGameCreditsHandlerNatsConst.Update,
            data: publish,
            cancellationToken: cancellationToken);
    }
}
