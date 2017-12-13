using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Serilog;

namespace ImapServer
{
    public class ImapServer
    {
        private TcpListener _imapListener;
        private ImapCommandParser _parser = new ImapCommandParser();

        public int Port
        {
            get
            {
                return ((IPEndPoint)_imapListener.LocalEndpoint).Port;
            }
        }

        public ImapServer(int port = 143)
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            _imapListener = new TcpListener(localAddr, port);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .MinimumLevel.Verbose()
                .CreateLogger();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var task = new Task(() =>
            {
                try
                {
                    _imapListener.Start();

                    // Enter the listening loop.
                    while (true)
                    {
                        // Console.Write("Waiting for a connection... ");

                        // Perform a blocking call to accept requests.
                        // You could also user server.AcceptSocket() here.
                        TcpClient client = _imapListener.AcceptTcpClient();
                        new Thread(() => HandleClient(client)).Start();
                    }
                }
                catch (SocketException e)
                {
                    Log.Logger.Fatal("SocketException: {exception}", e);
                }
                finally
                {
                    // Stop listening for new clients.
                    _imapListener.Stop();
                }

            }, cancellationToken);

            task.Start();

            return task;
        }

        private void HandleClient(TcpClient client)
        {
            var session = new ImapSession(client);
            session.Sent += Session_Sent;
            session.Recieved += Session_Recieved;
            session.HandleSession(CancellationToken.None);
        }

        private void Session_Recieved(object sender, string e)
        {
            Log.Logger.Verbose("[{contextId}] Recieved: {data}", ((ImapSessionContext)sender).ContextId, e);
        }

        private void Session_Sent(object sender, string e)
        {
            Log.Logger.Verbose("[{contextId}] Sent: {data}", ((ImapSessionContext)sender).ContextId, e);
        }
    }
}
