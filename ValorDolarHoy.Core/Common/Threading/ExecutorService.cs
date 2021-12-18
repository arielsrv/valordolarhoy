using Microsoft.VisualStudio.Threading;
using Polly;
using Polly.Bulkhead;
using System;
using System.Threading.Tasks;

namespace ValorDolarHoy.Core.Common.Threading;

public class ExecutorService
{
    private readonly BulkheadPolicy bulkheadPolicy;

    private ExecutorService(int size)
    {
        this.bulkheadPolicy = Policy.Bulkhead(size);
    }

    public static ExecutorService NewFixedThreadPool(int size)
    {
        return new ExecutorService(size);
    }

    public void Run(Action action)
    {
        Task.Run(() => { this.bulkheadPolicy.Execute(action); }).Forget();
    }
}