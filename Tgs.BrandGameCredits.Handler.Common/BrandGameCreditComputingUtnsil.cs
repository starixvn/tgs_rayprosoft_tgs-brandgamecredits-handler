using Tgs.BrandGameCredits.Handler.Common.Models;

namespace Tgs.BrandGameCredits.Handler.Common;

internal class BrandGameCreditComputingUtnsil : IBrandGameCreditComputingUtnsil
{
    public CreditCalculationInfo CreditCalculation(decimal initialAmount, decimal creditAmount, decimal debitAmount, decimal profitAmount)
    {
        var creditLimit = initialAmount + creditAmount - debitAmount;
        var creditAvailable = creditLimit;
        var creditAvailablePer = 100m;
        var creditUsed = 0.0m;
        var creditUsedPer = 0.0m;

        if (creditLimit + profitAmount < 0)
        {
            creditLimit += profitAmount;
            creditAvailable = 0;
            creditAvailablePer = 0;
            creditUsed = creditLimit;
            creditUsedPer = 100m;
        }
        else
        {
            if (creditLimit + profitAmount < creditLimit)
            {
                creditAvailable = creditLimit + profitAmount;
                creditAvailablePer = decimal.Parse((creditAvailable / creditLimit * 100).ToString("#.0000"));
                creditUsed = creditLimit - creditAvailable;
                creditUsedPer = 100m - creditAvailablePer;
            }
        }

        return new CreditCalculationInfo(
            CreditLimit: creditLimit,
            CreditUsed: creditUsed,
            CreditUsedPer: creditUsedPer,
            CreditAvailable: creditAvailable,
            CreditAvailablePer: creditAvailablePer);
    }
}
