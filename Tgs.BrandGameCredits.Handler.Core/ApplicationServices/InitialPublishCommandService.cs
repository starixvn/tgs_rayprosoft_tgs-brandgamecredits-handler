using Tgs.BrandGameCredits.Handler.Common;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class InitialPublishCommandService(
	ICommandService<PublishCommand> service) : ICommandService<InitialPublishCommand>
{
	public async ValueTask ExecuteAsync(InitialPublishCommand command, CancellationToken cancellationToken = default)
	{
		var tasks = new Task[] {
				Task.Run(async () => await service.ExecuteAsync(
					command: new PublishCommand(
						Subject: BrandGameCreditsHandlerNatsConst.Credit,
						ResellerId: command.ResellerId,
						ResellerCode: command.ResellerCode,
						LicenseeId: command.LicenseeId,
						LicenseeCode: command.LicenseeCode,
						BrandId: command.BrandId,
						BrandCode: command.BrandCode,
						CurrentTime: command.CurrentTime),
					cancellationToken:cancellationToken)
					.ConfigureAwait (false),
					cancellationToken:cancellationToken),
				Task.Run(async () => await service.ExecuteAsync(
					command: new PublishCommand(
						Subject: BrandGameCreditsHandlerNatsConst.Debit,
						ResellerId: command.ResellerId,
						ResellerCode: command.ResellerCode,
						LicenseeId: command.LicenseeId,
						LicenseeCode: command.LicenseeCode,
						BrandId: command.BrandId,
						BrandCode: command.BrandCode,
						CurrentTime: command.CurrentTime),
					cancellationToken:cancellationToken)
					.ConfigureAwait (false),
					cancellationToken:cancellationToken),
				Task.Run(async () => await service.ExecuteAsync(
					command: new PublishCommand(
						Subject: BrandGameCreditsHandlerNatsConst.Profit,
						ResellerId: command.ResellerId,
						ResellerCode: command.ResellerCode,
						LicenseeId: command.LicenseeId,
						LicenseeCode: command.LicenseeCode,
						BrandId: command.BrandId,
						BrandCode: command.BrandCode,
						CurrentTime: command.CurrentTime),
					cancellationToken:cancellationToken)
					.ConfigureAwait (false),
					cancellationToken:cancellationToken)
			};
		await Task.WhenAll(tasks).ConfigureAwait(false);
	}
}
