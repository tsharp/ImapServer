using DarkSpace.ImapServer.Core;
using DarkSpace.MailService.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DarkSpace.ImapServer
{
    public class ImapMailService : IHostedService
    {
        private static readonly ActivitySource ImapServerActivitySource = new ActivitySource("DarkSpace.ImapServer");
        private TcpListener imapListener;
        private IMailService mailService;
        private Task serverTask;

        public int Port
        {
            get
            {
                return ((IPEndPoint)imapListener.LocalEndpoint).Port;
            }
        }

        public ImapMailService(IMailService mailService, IOptions<ImapMailServiceSettings> options)
        {
            this.mailService = mailService;
            var settings = options.Value;
            IPAddress allIpv4Assigned = IPAddress.Parse(settings.IpAddress);
            imapListener = new TcpListener(allIpv4Assigned, settings.Port);
        }

        //public ImapMailService(IMailService mailService, int port = 143)
        //{
        //    this.mailService = mailService;
        //    IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        //    imapListener = new TcpListener(localAddr, port);
        //}

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if(serverTask != null)
            {
                return;
            }

            serverTask = Task.Run(() =>
            {
                using (var activity = ImapServerActivitySource.StartActivity("DarkSpace.ImapServer"))
                {
                    activity.Start();

                    try
                    {
                        imapListener.Start();

                        // Enter the listening loop.
                        while (true)
                        {
                            // Console.Write("Waiting for a connection... ");

                            // Perform a blocking call to accept requests.
                            // You could also user server.AcceptSocket() here.
                            var clientTask = imapListener.AcceptTcpClientAsync();
                            clientTask.Wait(cancellationToken);
                            Task.Run(() => HandleClient(clientTask.Result, cancellationToken), cancellationToken);
                        }
                    }
                    catch (SocketException ex)
                    {
                        activity.RecordException(ex);
                        activity.SetStatus(Status.Error.WithDescription(ex.Message));
                    }
                    finally
                    {
                        // Stop listening for new clients.
                        imapListener.Stop();
                    }

                    activity.Stop();
                }

            }, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        private void HandleClient(TcpClient client, CancellationToken cancellationToken)
        {
            using (var activity = ImapServerActivitySource.StartActivity("ImapServer.Session", ActivityKind.Server))
            {
                try
                {
                    var session = new ImapSession(client, mailService);
                    activity.SetTag("context.id", session.ContextId);
                    session.Sent += Session_Sent;
                    session.Recieved += Session_Recieved;
                    session.HandleSession(cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
                finally
                {
                    client.Close();
                    client.Dispose();
                }
            }
        }

        private void Session_Recieved(object sender, string e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Recieved: {e}");
            Console.ForegroundColor = ConsoleColor.White;
            Activity.Current?.AddEvent(new ActivityEvent(
                "log",
                DateTime.UtcNow,
                new ActivityTagsCollection(
                    new Dictionary<string, object>
                    {
                        { "log.severity", "verbose" },
                        { "log.message", $"[{((ImapSessionContext)sender).ContextId}] Recieved: {e}" }
                    }
                )
            ));

            //Log.Logger.Verbose("[{contextId}] Recieved: {data}", ((ImapSessionContext)sender).ContextId, e);
        }

        private void Session_Sent(object sender, string e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Sent: {e}");
            Console.ForegroundColor = ConsoleColor.White;

            Activity.Current?.AddEvent(new ActivityEvent(
                "log",
                DateTime.UtcNow,
                new ActivityTagsCollection(
                    new Dictionary<string, object>
                    {
                        { "log.severity", "verbose" },
                        { "log.message", $"[{((ImapSessionContext)sender).ContextId}] Sent: {e}" }
                    }
                )
            ));
        }
    }
}
