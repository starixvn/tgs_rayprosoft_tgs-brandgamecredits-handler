using System.ComponentModel.DataAnnotations;

namespace Tgs.BrandGameCredits.Handler.ViewModels;

public record CreditViewModel
{
	[Required]
	public string BrandCode { get; init; } = default!;
}
