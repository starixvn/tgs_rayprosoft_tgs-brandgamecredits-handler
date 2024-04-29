using Dapper.ColumnMapper;

namespace Tgs.BrandGameCredits.Handler.Core.Models;

public record BrandInfo
{
	[ColumnMapping(columnName: "reseller_id")]
	public Guid ResellerId { get; set; }

	[ColumnMapping(columnName: "reseller_code")]
	public string ResellerCode { get; set; } = default!;

	[ColumnMapping(columnName: "licensee_id")]
	public Guid LicenseeId { get; set; }

	[ColumnMapping(columnName: "licensee_code")]
	public string LicenseeCode { get; set; } = default!;

	[ColumnMapping(columnName: "brand_id")]
	public Guid BrandId { get; set; }

	[ColumnMapping(columnName: "brand_code")]
	public string BrandCode { get; set; } = default!;
	[ColumnMapping(columnName: "time_span_minutes")]
	public int TimeSpanMinutes { get; set; } = default!;
}
