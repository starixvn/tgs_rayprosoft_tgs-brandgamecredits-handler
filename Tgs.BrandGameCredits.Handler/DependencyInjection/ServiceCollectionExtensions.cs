using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBrandGameCredits(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> tgsConfigure)
		=> services
		.AddBrandGameCreditsCore()
		.AddBrandGameCreditsRepositories(tgsConfigure);
}
