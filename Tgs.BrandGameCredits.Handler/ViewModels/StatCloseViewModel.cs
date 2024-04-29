using System.ComponentModel.DataAnnotations;

namespace Tgs.BrandGameCredits.Handler.ViewModels;

public record StatCloseViewModel
{
	[Required]
	public string BrandCode { get; init; } = default!;
}
