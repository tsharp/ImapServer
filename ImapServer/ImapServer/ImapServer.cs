using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ImapServer
{
    public class ImapServer
    {
        private static readonly ActivitySource ImapServerActivitySource = new ActivitySource("DarkSpace.ImapServer");

        private TcpListener imapListener;
        private ImapCommandParser parser = new ImapCommandParser();

        public int Port
        {
            get
            {
                return ((IPEndPoint)imapListener.LocalEndpoint).Port;
            }
        }

        public ImapServer(int port = 143)
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            imapListener = new TcpListener(localAddr, port);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var task = new Task(() =>
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
                            new Thread(() => HandleClient(clientTask.Result, cancellationToken)).Start();
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

            task.Start();

            return task;
        }

        private void HandleClient(TcpClient client, CancellationToken cancellationToken)
        {
            using (var activity = ImapServerActivitySource.StartActivity("ImapServer.Session", ActivityKind.Server))
            {
                try
                {
                    var session = new ImapSession(client);
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
            Console.WriteLine("Recieved ...");
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
            Console.WriteLine("Data Sent ...");
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
