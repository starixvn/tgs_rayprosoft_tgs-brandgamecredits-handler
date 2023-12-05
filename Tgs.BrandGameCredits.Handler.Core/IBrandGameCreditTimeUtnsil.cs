namespace Tgs.BrandGameCredits.Handler.Core;

public interface IBrandGameCreditTimeUtnsil
{
    DateTimeOffset GetTimeZone(DateTimeOffset currentTime, int timeSpanMinutes = 480);
    DateOnly GetDateOnly(DateTimeOffset currentTime, int timeSpanMinutes = 480);
}
