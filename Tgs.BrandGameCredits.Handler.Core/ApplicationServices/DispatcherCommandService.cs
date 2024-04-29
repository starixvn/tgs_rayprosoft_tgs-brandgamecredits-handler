using Tgs.BrandGameCredits.Handler.Common;
using Tgs.ServiceUtility;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

internal class DispatcherCommandService(
	ICommandService<CreateCommand> createService,
	ICommandService<InitialCommand> initialService,
	ICommandService<CreditCommand> creditService,
	ICommandService<DebitCommand> debitService,
	ICommandService<ProfitCommand> profitService,
	ICommandService<SettlementCommand> settlementService) : ICommandService<DispatcherCommand>
{
	public ValueTask ExecuteAsync(DispatcherCommand command, CancellationToken cancellationToken = default)
	=> command.DispatcherType switch
	{
		DispatcherType.Create => createService.ExecuteAsync(
			command: new CreateCommand(
				command.ResellerId,
				command.ResellerCode,
				command.LicenseeId,
				command.LicenseeCode,
				command.BrandId,
				command.BrandCode,
				command.CurrentTime),
			cancellationToken: cancellationToken),
		DispatcherType.Initial => initialService.ExecuteAsync(
			command: new InitialCommand(
				command.ResellerId,
				command.ResellerCode,
				command.LicenseeId,
				command.LicenseeCode,
				command.BrandId,
				command.BrandCode,
				command.CurrentTime),
			cancellationToken: cancellationToken),
		DispatcherType.Credit => creditService.ExecuteAsync(
			command: new CreditCommand(
				command.ResellerId,
				command.ResellerCode,
				command.LicenseeId,
				command.LicenseeCode,
				command.BrandId,
				command.BrandCode,
				command.CurrentTime),
			cancellationToken: cancellationToken),
		DispatcherType.Debit => debitService.ExecuteAsync(
			command: new DebitCommand(
				command.ResellerId,
				command.ResellerCode,
				command.LicenseeId,
				command.LicenseeCode,
				command.BrandId,
				command.BrandCode,
				command.CurrentTime),
			cancellationToken: cancellationToken),
		DispatcherType.Profit => profitService.ExecuteAsync(
			command: new ProfitCommand(
				command.ResellerId,
				command.ResellerCode,
				command.LicenseeId,
				command.LicenseeCode,
				command.BrandId,
				command.BrandCode,
				command.CurrentTime),
			cancellationToken: cancellationToken),
		DispatcherType.Settlement => settlementService.ExecuteAsync(
			command: new SettlementCommand(
				command.ResellerId,
				command.ResellerCode,
				command.LicenseeId,
				command.LicenseeCode,
				command.BrandId,
				command.BrandCode,
				command.CurrentTime),
			cancellationToken: cancellationToken),
		_ => throw new NotImplementedException()
	};
}
