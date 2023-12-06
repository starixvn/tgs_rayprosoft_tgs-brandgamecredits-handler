using Tgs.BrandGameCredits.Handler.Common;

namespace Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

public record DispatcherCommand(
    Guid ResellerId,
    string ResellerCode,
    Guid LicenseeId,
    string LicenseeCode,
    Guid BrandId,
    string BrandCode,
    DateTimeOffset CurrentTime,
    DispatcherType DispatcherType);
