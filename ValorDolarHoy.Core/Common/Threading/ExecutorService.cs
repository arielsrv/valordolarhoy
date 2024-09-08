using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using Polly;
using Polly.Bulkhead;

#pragma warning disable CS1591

namespace ValorDolarHoy.Core.Common.Threading;

public class ExecutorService(int size)
{
    private readonly BulkheadPolicy _bulkheadPolicy = Policy.Bulkhead(size);

    public void Run(Action action)
    {
        Task.Run(() => { this._bulkheadPolicy.Execute(action); }).Forget();
    }
}

public static class Executors
{
    public static ExecutorService NewFixedThreadPool(int size)
    {
        return new ExecutorService(size);
    }
}
