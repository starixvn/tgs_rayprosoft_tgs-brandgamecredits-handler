using System.Data;
using System.Runtime.CompilerServices;
using Dapper;
using Tgs.BrandGameCredits.Handler.Core;
using Tgs.BrandGameCredits.Handler.Core.Models;

namespace Tgs.BrandGameCredits.Handler.Repositories;

internal class BrandRepository(IDbConnection connection) : IBrandRepository
{
	public async ValueTask<BrandInfo?> GetAsync(string code, CancellationToken cancellationToken = default)
	{
		DynamicParameters parameters = new DynamicParameters();
		parameters.Add("@code", code, DbType.String, ParameterDirection.Input);

		var sqlQuery = @"
        select
	        r.id as ""reseller_id"",
	        r.code as ""reseller_code"",
	        l.id as ""licensee_id"",
	        l.code as ""licensee_code"",
	        b.id as ""brand_id"",
	        b.code as ""brand_code"",
            b.time_span_minutes as ""time_span_minutes""
        from
	        dbo.brands b
        join dbo.licensees l
        on
	        b.licensee_id = l.id
        join dbo.resellers r
        on
	        l.reseller_id = r.id
        where
	        b.status != 2
	        and l.status != 2
	        and r.status != 2
	        and b.code = @code;";

		var command = new CommandDefinition(
			commandText: sqlQuery,
			cancellationToken: cancellationToken);

		return await connection
			.QuerySingleOrDefaultAsync<BrandInfo>(
			command: command)
			.ConfigureAwait(false);
	}

	public async IAsyncEnumerable<BrandInfo> QueryAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var sqlQuery = @"
        select
	        r.id as ""reseller_id"",
	        r.code as ""reseller_code"",
	        l.id as ""licensee_id"",
	        l.code as ""licensee_code"",
	        b.id as ""brand_id"",
	        b.code as ""brand_code"",
            b.time_span_minutes as ""time_span_minutes""
        from
	        dbo.brands b
        join dbo.licensees l
        on
	        b.licensee_id = l.id
        join dbo.resellers r
        on
	        l.reseller_id = r.id
        where
	        b.status != 2
	        and l.status != 2
	        and r.status != 2;"
		;

		var command = new CommandDefinition(
			commandText: sqlQuery,
			cancellationToken: cancellationToken);

		var infos = await connection.QueryAsync<BrandInfo>(
			command: command)
			.ConfigureAwait(false);

		if (!infos.Any())
		{
			yield break;
		}

		foreach (var info in infos)
		{
			yield return info;
		}
	}
}
