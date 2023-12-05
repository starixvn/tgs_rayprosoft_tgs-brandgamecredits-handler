using Microsoft.EntityFrameworkCore;
using Tgs.BrandGameCredits.Handler.Core;
using Tgs.BrandGameCredits.Handler.Repositories;
using Tgs.BrandGameCredits.Handler.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddBrandGameCreditsRepositories(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> tgsConfigure)
        => services
        .AddDbContext<TgsContext>(tgsConfigure)
        .AddTransient<IGameWagerStatDailyCloseRepository, GameWagerStatDailyCloseRepository>()
        .AddTransient<IBrandGameCreditRepository, BrandGameCreditRepository>();
}
