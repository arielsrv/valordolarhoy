using System.Threading;
using ValorDolarHoy.Core.Common.Threading;
using Xunit;

namespace ValorDolarHoy.Test.Unit.Common.Threading;

public class ExecutorServiceTest
{
    [Fact]
    public void Fire_And_Forget()
    {
        ExecutorService executorService = Executors.NewFixedThreadPool(10);

        int value = 0;
        executorService.Run(() => { value = int.MaxValue; });

        Assert.Equal(0, value);
    }

    [Fact]
    public void Fire_And_Forget_Expected_Value()
    {
        ExecutorService executorService = Executors.NewFixedThreadPool(10);

        int value = 0;
        executorService.Run(() =>
        {
            Thread.Sleep(50);
            value = int.MaxValue;
        });

        Thread.Sleep(100);
        Assert.Equal(int.MaxValue, value);
    }
}