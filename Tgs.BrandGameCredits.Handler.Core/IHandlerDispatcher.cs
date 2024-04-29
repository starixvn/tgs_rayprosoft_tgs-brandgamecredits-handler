using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

namespace Tgs.BrandGameCredits.Handler.Core;

public interface IHandlerDispatcher
{
	ValueTask PushCommandAsync(DispatcherCommand command, CancellationToken cancellationToken = default);
}
