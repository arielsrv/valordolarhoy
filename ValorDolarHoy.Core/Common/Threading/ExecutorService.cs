using Microsoft.VisualStudio.Threading;
using Polly;
using Polly.Bulkhead;
using System;
using System.Threading.Tasks;

namespace ValorDolarHoy.Core.Common.Threading;

public class ExecutorService
{
    private readonly BulkheadPolicy bulkheadPolicy;

    public ExecutorService(int size)
    {
        this.bulkheadPolicy = Policy.Bulkhead(size);
    }

    public void Run(Action action)
    {
        Task.Run(() => { this.bulkheadPolicy.Execute(action); }).Forget();
    }
}

public static class Executors
{
    public static ExecutorService NewFixedThreadPool(int size)
    {
        return new ExecutorService(size);
    }
}