using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.BrandGameCredits.Handler.Core;

public interface IBrandGameCreditTxnRepository
{
    public ValueTask<decimal> SumCreditAsync(Guid brandId, DateTimeOffset createdOnStart, DateTimeOffset createdOnEnd, CancellationToken cancellationToken);
    public ValueTask<decimal> SumDebitAsync(Guid brandId, DateTimeOffset createdOnStart, DateTimeOffset createdOnEnd, CancellationToken cancellationToken);
}
