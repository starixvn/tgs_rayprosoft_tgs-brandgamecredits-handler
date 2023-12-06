using Tgs.BrandGameCredits.Handler.Core.Models;

namespace Tgs.BrandGameCredits.Handler.Core;

public interface IBrandRepository
{
    ValueTask<BrandInfo?> GetAsync(string code, CancellationToken cancellationToken = default);
}
