using Tgs.BrandGameCredits.Handler.Common;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBrandGameCreditsHandlerCommon(
		this IServiceCollection services)
		=> services
		.AddSingleton<IBrandGameCreditComputingUtnsil, BrandGameCreditComputingUtnsil>()
		.AddSingleton<IBrandGameCreditTimeUtnsil, BrandGameCreditTimeUtnsil>();
}
