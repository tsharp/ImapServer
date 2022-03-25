using MailKit.Examples;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Threading;

namespace ImapServer.CoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddSource("DarkSpace.ImapServer")
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: "DarkSpace.ImapServer", serviceVersion: "1.0.0.0"))
                .AddConsoleExporter()
                .Build())
            {
                var cancellationTokenSource = new CancellationTokenSource();
                
                ImapServer server = new ImapServer();
                var serverTask = server.StartAsync(cancellationTokenSource.Token);

                while (true)
                {
                    tracerProvider.ForceFlush();

                    try
                    {
                        ImapExamples.DownloadMessages_v1();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    System.Threading.Thread.Sleep(1000);
                }
            }
        }
    }
}
