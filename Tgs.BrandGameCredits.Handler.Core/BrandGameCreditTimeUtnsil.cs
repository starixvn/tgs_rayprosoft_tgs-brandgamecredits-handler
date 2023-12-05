using Microsoft.Extensions.Options;
using Tgs.BrandGameCredits.Handler.Core;

namespace Tgs.BrandGameCredits.Handler.Core;

internal class BrandGameCreditTimeUtnsil : IBrandGameCreditTimeUtnsil
{
    public DateOnly GetDateOnly(DateTimeOffset currentTime, int timeSpanMinutes = 480)
    {
        var timeSpan = TimeSpan.FromMinutes(timeSpanMinutes);
        var currentTimeOffset = currentTime.ToOffset(timeSpan);
        return DateOnly.FromDateTime(new DateTimeOffset(currentTimeOffset.Year, currentTimeOffset.Month, 1, 0, 0, 0, timeSpan).Date);
    }

    public DateTimeOffset GetTimeZone(DateTimeOffset currentTime, int timeSpanMinutes = 480)
    {
        var timeSpan = TimeSpan.FromMinutes(timeSpanMinutes);
        var currentTimeOffset = currentTime.ToOffset(timeSpan);
        return new DateTimeOffset(currentTimeOffset.Year, currentTimeOffset.Month, 1, 0, 0, 0, timeSpan);
    }
}
