using Tgs.BrandGameCredits.Handler.Common;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class StatCloseCommandService(
	ISystemClock systemClock,
	ICommandService<PublishCommand> service) : ICommandService<StatCloseCommand>
{
	public async ValueTask ExecuteAsync(StatCloseCommand command, CancellationToken cancellationToken = default)
	{
		var currentTime = systemClock.CurrentTime;

		await service.ExecuteAsync(
			command: new PublishCommand(
				Subject: BrandGameCreditsHandlerNatsConst.Profit,
				ResellerId: command.ResellerId,
				ResellerCode: command.ResellerCode,
				LicenseeId: command.LicenseeId,
				LicenseeCode: command.LicenseeCode,
				BrandId: command.BrandId,
				BrandCode: command.BrandCode,
				CurrentTime: currentTime),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
