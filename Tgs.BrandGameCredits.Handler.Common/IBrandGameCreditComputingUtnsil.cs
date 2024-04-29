using Tgs.BrandGameCredits.Handler.Common.Models;

namespace Tgs.BrandGameCredits.Handler.Common;

public interface IBrandGameCreditComputingUtnsil
{
	CreditCalculationInfo CreditCalculation(decimal initialAmount, decimal creditAmount, decimal debitAmount, decimal profitAmount);
}
