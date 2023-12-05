namespace Tgs.BrandGameCredits.Handler.Core;

public interface ISystemClock
{
    DateTimeOffset CurrentTime { get; }
}
