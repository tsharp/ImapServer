using DarkSpace.MailService.Abstractions;
using MailKit.Examples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Threading;

namespace DarkSpace.ImapServer.CoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (TracerProvider tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddSource("DarkSpace.ImapServer")
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: "DarkSpace.ImapServer", serviceVersion: "1.0.0.0"))
                .AddConsoleExporter()
                .Build())
            {
                Host.CreateDefaultBuilder(args)
                    .ConfigureServices(services =>
                    {
                        services.AddOptions();
                        services.Configure<ImapMailServiceSettings>(settings =>
                        {
                            settings.Port = 143;
                            settings.IpAddress = "127.0.0.1";
                        });

                        services.AddSingleton(tracerProvider);
                        services.AddSingleton<IMailService, MailService.MailService>();
                        services.AddHostedService<ImapMailService>();
                    })
                    .Build()
                    .Run();
            }
        }
    }
}
