namespace Tgs.BrandGameCredits.Handler.Common.UnitTest;

public class GameCreditComputingUtnsilUnitTest
{
    [Fact]
    public void Credit_Available()
    {
        var utnsil = new BrandGameCreditComputingUtnsil();

        var actual = utnsil.CreditCalculation(2, 100, 1, 33);

        Assert.Equal(101m, actual.CreditLimit);
        Assert.Equal(33m, actual.CreditUsed);
        Assert.Equal(32.6733m, actual.CreditUsedPer);
        Assert.Equal(68m, actual.CreditAvailable);
        Assert.Equal(67.3267m, actual.CreditAvailablePer);
    }

    [Fact]
    public void The_Brand_Credit_Is_Positive_1000_And_The_Player_Profit_Is_Negative_1001()
    {
        /*
         * 此品牌信用正1000玩家利潤為負1001
         * 此品牌利潤為正1001
         */
        var utnsil = new BrandGameCreditComputingUtnsil();

        var actual = utnsil.CreditCalculation(initialAmount: 99, creditAmount: 902, debitAmount: 1, profitAmount: 1001);

        Assert.Equal(-1m, actual.CreditLimit);
        Assert.Equal(-1m, actual.CreditUsed);
        Assert.Equal(100m, actual.CreditUsedPer);
        Assert.Equal(0, actual.CreditAvailable);
        Assert.Equal(0, actual.CreditAvailablePer);
    }

    [Fact]
    public void The_Brand_Credit_Is_Negative_1000_And_The_Player_Profit_Is_Negative_333()
    {
        /*
         * 此品牌信用負1000玩家利潤為負333
         * 此品牌利潤為正333
         */
        var utnsil = new BrandGameCreditComputingUtnsil();

        var actual = utnsil.CreditCalculation(initialAmount: -500, creditAmount: 1000, debitAmount: 1500, profitAmount: 333);

        Assert.Equal(-1333m, actual.CreditLimit);
        Assert.Equal(-1333m, actual.CreditUsed);
        Assert.Equal(100m, actual.CreditUsedPer);
        Assert.Equal(0, actual.CreditAvailable);
        Assert.Equal(0, actual.CreditAvailablePer);
    }
}
