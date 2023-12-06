using NSubstitute;

namespace Tgs.BrandGameCredits.Handler.Core.UnitTest;

public class BrandGameCreditTimeUtnsilUnitTest
{
    [Fact]
    public void Get_1st_Of_Every_Month_Time_Zone_23_11_30_17_59_59_0000()
    {
        var currentTime = DateTimeOffset.Parse("2023-11-30T15:59:59+00:00");

        var utnsil = new BrandGameCreditTimeUtnsil();

        var month = utnsil.Get1stOfEveryMonthTimeZone(currentTime, 480);

        Assert.Equal(DateTimeOffset.Parse("2023-11-01T00:00:00+08:00").DateTime, month.DateTime);
    }

    [Fact]
    public void Get_1st_Of_Every_Month_Time_Zone_23_11_30_16_00_00_0000()
    {
        var currentTime = DateTimeOffset.Parse("2023-11-30T16:00:00+00:00");

        var utnsil = new BrandGameCreditTimeUtnsil();

        var month = utnsil.Get1stOfEveryMonthTimeZone(currentTime, 480);

        Assert.Equal(DateTimeOffset.Parse("2023-12-01T00:00:00+08:00").DateTime, month.DateTime);
    }

    [Fact]
    public void Get_1st_Of_Every_Month_Time_Zone_23_11_30_23_59_59_0800()
    {
        var currentTime = DateTimeOffset.Parse("2023-11-30T23:59:59+08:00");

        var utnsil = new BrandGameCreditTimeUtnsil();

        var month = utnsil.Get1stOfEveryMonthTimeZone(currentTime, 480);

        Assert.Equal(DateTimeOffset.Parse("2023-11-01T00:00:00+08:00").DateTime, month.DateTime);
    }

    [Fact]
    public void Get_1st_Of_Every_Month_Time_Zone_23_12_01_00_00_0800()
    {
        var currentTime = DateTimeOffset.Parse("2023-12-01T00:00:00+08:00");

        var utnsil = new BrandGameCreditTimeUtnsil();

        var month = utnsil.Get1stOfEveryMonthTimeZone(currentTime, 480);

        Assert.Equal(DateTimeOffset.Parse("2023-12-01T00:00:00+08:00").DateTime, month.DateTime);
    }

    [Fact]
    public void Get_1st_Of_Every_Month_Time_Zone_23_11_30_21_29_59_0530()
    {
        var currentTime = DateTimeOffset.Parse("2023-11-30T21:29:59+05:30");

        var utnsil = new BrandGameCreditTimeUtnsil();

        var month = utnsil.Get1stOfEveryMonthTimeZone(currentTime, 480);

        Assert.Equal(DateTimeOffset.Parse("2023-11-01T00:00:00+08:00").DateTime, month.DateTime);
    }

    [Fact]
    public void Get_1st_Of_Every_Month_Time_Zone_23_11_30_21_30_00_0530()
    {
        var currentTime = DateTimeOffset.Parse("2023-11-30T21:30:00+05:30");

        var utnsil = new BrandGameCreditTimeUtnsil();

        var month = utnsil.Get1stOfEveryMonthTimeZone(currentTime, 480);

        Assert.Equal(DateTimeOffset.Parse("2023-12-01T00:00:00+08:00").DateTime, month.DateTime);
    }
}