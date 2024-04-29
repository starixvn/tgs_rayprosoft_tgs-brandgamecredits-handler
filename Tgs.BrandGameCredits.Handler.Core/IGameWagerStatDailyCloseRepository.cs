using Tgs.BrandGameCredits.Handler.Core.Models;

namespace Tgs.BrandGameCredits.Handler.Core;

public interface IGameWagerStatDailyCloseRepository
{
	public IAsyncEnumerable<GameWagerStatDailyCloseInfo> QueryAsync(Guid brandId, DateTimeOffset statOnStart, DateTimeOffset statOnEnd, DateOnly dayStart, DateOnly dayEnd, CancellationToken cancellationToken = default);
}
