using System.ComponentModel.DataAnnotations;

namespace Tgs.BrandGameCredits.Handler.ViewModels;

public record CreateViewModel
{
	[Required]
	public string BrandCode { get; init; } = default!;
	[DataType(DataType.DateTime)]
	[Required]
	public DateTimeOffset MonthTime { get; init; } = default!;
}
