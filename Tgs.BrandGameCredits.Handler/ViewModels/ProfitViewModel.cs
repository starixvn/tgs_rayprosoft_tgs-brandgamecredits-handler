using System.ComponentModel.DataAnnotations;

namespace Tgs.BrandGameCredits.Handler.ViewModels;

public record ProfitViewModel
{
	[Required]
	public string BrandCode { get; init; } = default!;
}
