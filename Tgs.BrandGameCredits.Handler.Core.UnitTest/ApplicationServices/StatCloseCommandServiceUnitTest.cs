using NSubstitute;
using Tgs.BrandGameCredits.Handler.Common;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.UnitTest.ApplicationServices;

public class StatCloseCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeSystemClock = Substitute.For<ISystemClock>();
		var fakePublishService = Substitute.For<ICommandService<PublishCommand>>();

		var command = new StatCloseCommand(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI");

		var currentTime = new DateTimeOffset(2023, 12, 06, 10, 55, 26, TimeSpan.FromMinutes(480));
		_ = fakeSystemClock.CurrentTime.Returns(currentTime);

		var service = new StatCloseCommandService(
			fakeSystemClock,
			fakePublishService);

		await service.ExecuteAsync(command: command);

		_ = fakePublishService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new PublishCommand(
				Subject: BrandGameCreditsHandlerNatsConst.Profit,
				ResellerId: command.ResellerId,
				ResellerCode: command.ResellerCode,
				LicenseeId: command.LicenseeId,
				LicenseeCode: command.LicenseeCode,
				BrandId: command.BrandId,
				BrandCode: command.BrandCode,
				CurrentTime: currentTime)),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
