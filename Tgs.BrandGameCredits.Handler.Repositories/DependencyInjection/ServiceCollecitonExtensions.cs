using System.Data;
using Dapper;
using Dapper.ColumnMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Tgs.BrandGameCredits.Handler.Core;
using Tgs.BrandGameCredits.Handler.Core.Models;
using Tgs.BrandGameCredits.Handler.Repositories;
using Tgs.BrandGameCredits.Handler.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddBrandGameCreditsRepositories(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> tgsConfigure)
    {
        _ = services
        .AddDbContext<TgsContext>(tgsConfigure)
        .AddScoped<IDbConnection, NpgsqlConnection>((sp) => (NpgsqlConnection)sp.GetRequiredService<TgsContext>().Database.GetDbConnection())
        .AddTransient<IBrandGameCreditRepository, BrandGameCreditRepository>()
        .AddTransient<IBrandGameCreditTxnRepository, BrandGameCreditTxnRepository>()
        .AddTransient<IBrandRepository, BrandRepository>()
        .AddTransient<IGameWagerStatDailyCloseRepository, GameWagerStatDailyCloseRepository>();

        SqlMapper.SetTypeMap(typeof(BrandInfo), new ColumnTypeMapper(typeof(BrandInfo)));

        return services;
    }
}
