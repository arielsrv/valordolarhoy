using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using Polly;
using Polly.Bulkhead;

namespace ValorDolarHoy.Common.Threading
{
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
            Task
                .Run(() => { bulkheadPolicy.Execute(action); })
                .Forget();
        }
    }
}