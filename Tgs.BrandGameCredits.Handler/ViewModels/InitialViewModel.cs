using System.ComponentModel.DataAnnotations;

namespace Tgs.BrandGameCredits.Handler.ViewModels;

public record InitialViewModel
{
    [Required]
    public string BrandCode { get; init; } = default!;
}
