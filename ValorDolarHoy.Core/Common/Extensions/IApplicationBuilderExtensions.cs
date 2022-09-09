using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Threading;

namespace ValorDolarHoy.Core.Common.Extensions;

public static class IApplicationBuilderExtensions
{
    public static void UseWarmUp(this IApplicationBuilder applicationBuilder)
    {
        Task.Run(() =>
            {
                IApplicationWarmUpper? applicationWarmUpper =
                    applicationBuilder.ApplicationServices.GetService<IApplicationWarmUpper>();

                if (applicationWarmUpper == null) return;

                WarmupExecutor.Warmup(applicationWarmUpper);
            }
        ).Forget();
    }
}