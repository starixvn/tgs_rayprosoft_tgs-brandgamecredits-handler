using System.ComponentModel.DataAnnotations;

namespace Tgs.BrandGameCredits.Handler.ViewModels;

public class CreateMonthViewModel
{
    [DataType(DataType.DateTime)]
    [Required]

    public DateTimeOffset MonthTime { get; init; } = default!;
}
