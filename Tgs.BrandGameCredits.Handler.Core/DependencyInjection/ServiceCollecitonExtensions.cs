using System.Reflection;
using Tgs.BrandGameCredits.Handler.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddBrandGameCreditsCore(
        this IServiceCollection services)
        => services
        .RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient)
        .AddSingleton<ISystemClock, SystemClock>()
        .AddSingleton<IBrandGameCreditTimeUtnsil, BrandGameCreditTimeUtnsil>();
}
