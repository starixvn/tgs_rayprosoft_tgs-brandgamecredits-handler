using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Common.Protos;
using Tgs.BrandGameCredits.Handler.Core;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Valhalla.MessageQueue;

namespace Tgs.BrandGameCredits.Handler.Messages;

public class SettlementMessageHandler(
	ILogger<SettlementMessageHandler> logger,
	IHandlerDispatcher dispatcher) : IMessageHandler<BrandGameCreditPublish>
{
	public ValueTask HandleAsync(string subject, BrandGameCreditPublish data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This receiving message data info.({data})",
			DateTime.Now.ToString("o"),
			subject,
			data);

		return dispatcher.PushCommandAsync(
			command: new DispatcherCommand(
				ResellerId: data.ResellerId.ToGuid(),
				ResellerCode: data.ResellerCode,
				LicenseeId: data.LicenseeId.ToGuid(),
				LicenseeCode: data.LicenseeCode,
				BrandId: data.BrandId.ToGuid(),
				BrandCode: data.BrandCode,
				CurrentTime: data.CurrentTime.ToDateTimeOffset(),
				DispatcherType: DispatcherType.Settlement),
			cancellationToken: cancellationToken);
	}
}
