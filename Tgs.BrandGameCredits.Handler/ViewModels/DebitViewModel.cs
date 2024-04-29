using System.ComponentModel.DataAnnotations;

namespace Tgs.BrandGameCredits.Handler.ViewModels;

public record DebitViewModel
{
	[Required]
	public string BrandCode { get; init; } = default!;
}
