using System.Threading;
using ValorDolarHoy.Common.Threading;
using Xunit;

namespace ValorDolarHoy.Test.Common.Threading;

public class ExecutorServiceTest
{
    private readonly ExecutorService executorService;

    public ExecutorServiceTest()
    {
        this.executorService = ExecutorService.NewFixedThreadPool(10);
    }

    [Fact]
    public void Fire_And_Forget()
    {
        int value = 0;
        this.executorService.Run(() =>
        {
            Thread.Sleep(100);
            value = int.MaxValue;
        });

        Assert.Equal(0, value);
    }

    [Fact]
    public void Fire_And_Forget_Expected_Value()
    {
        int value = 0;
        this.executorService.Run(() =>
        {
            Thread.Sleep(100);
            value = int.MaxValue;
        });

        Thread.Sleep(200);
        Assert.Equal(int.MaxValue, value);
    }
}