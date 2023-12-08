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
        .AddBrandGameCreditsHandlerCommon()
        .AddSingleton<ISystemClock, SystemClock>()
        .AddSingleton<IHandlerDispatcher, HandlerDispatcher>();
}
