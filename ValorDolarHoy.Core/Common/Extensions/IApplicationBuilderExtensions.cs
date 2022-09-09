using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.VisualStudio.Threading;

namespace ValorDolarHoy.Core.Common.Extensions;

public static class IApplicationBuilderExtensions
{
    public static void UseWarmUp(this IApplicationBuilder applicationBuilder,
        IApplicationWarmUpper applicationWarmUpper)
    {
        Task.Run(() =>
            {
                WarmupExecutor warmupExecutor = new(applicationWarmUpper);
                warmupExecutor.Warmup();
            }
        ).Forget();
    }
}