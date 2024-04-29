namespace Tgs.BrandGameCredits.Handler.Common;

public interface IBrandGameCreditTimeUtnsil
{
	DateTimeOffset Get1stOfEveryMonthTimeZone(DateTimeOffset currentTime, int timeSpanMinutes = 480);

	DateOnly Get1stOfEveryMonthDateOnly(DateTimeOffset currentTime, int timeSpanMinutes = 480);
}
