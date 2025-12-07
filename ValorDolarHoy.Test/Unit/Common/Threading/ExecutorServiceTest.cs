using System;
using System.Threading;
using ValorDolarHoy.Core.Common.Threading;
using Xunit;

namespace ValorDolarHoy.Test.Unit.Common.Threading;

public class ExecutorServiceTest
{
    [Fact]
    public void Fire_And_Forget()
    {
        ExecutorService executorService = Executors.NewFixedThreadPool(Environment.ProcessorCount - 1);

        var value = 0;
        executorService.Run(() =>
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            value = int.MaxValue;
        });

        Assert.Equal(0, value);
    }

    // [Fact]
    // public void Fire_And_Forget_Expected_Value()
    // {
    //     ExecutorService executorService = Executors.NewFixedThreadPool(Environment.ProcessorCount - 1);
    //
    //     var value = 0;
    //     executorService.Run(() => { value = int.MaxValue; });
    //
    //     Thread.Sleep(TimeSpan.FromMilliseconds(1000));
    //     Assert.Equal(int.MaxValue, value);
    // }
}