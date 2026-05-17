using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices;

#pragma warning disable CS1591

namespace ValorDolarHoy.Core.Common.Spa;

public static class ViteDevServer
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(5)
    };

    public static void UseViteDevelopmentServer(
        this ISpaBuilder spaBuilder,
        string workingDirectory,
        int port = 3000)
    {
        Uri viteUri = new Uri($"http://localhost:{port}");
        Task readyTask = StartViteAndWaitAsync(workingDirectory, port);

        // UseProxyToSpaDevelopmentServer(Func<Task<Uri>>) waits for the task
        // before forwarding the first request — no race condition.
        spaBuilder.UseProxyToSpaDevelopmentServer(() =>
            readyTask.ContinueWith(_ => viteUri, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.Default));
    }

    private static Task StartViteAsync(string workingDirectory, int port)
    {
        try
        {
            var (shell, args) = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? ("cmd", "/c npm start")
                : ("sh", "-c \"npm start\"");

            ProcessStartInfo psi = new()
            {
                FileName = shell,
                Arguments = args,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                Environment =
                {
                    ["PORT"] = port.ToString()
                }
            };

            Process.Start(psi);
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    private static async Task StartViteAndWaitAsync(string workingDirectory, int port)
    {
        await StartViteAsync(workingDirectory, port);

        DateTime deadline = DateTime.UtcNow.AddSeconds(120);

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                await HttpClient.GetAsync($"http://localhost:{port}");
                return; // Vite is ready
            }
            catch
            {
                await Task.Delay(500);
            }
        }

        throw new TimeoutException($"Vite dev server on port {port} did not start within 120 seconds.");
    }
}