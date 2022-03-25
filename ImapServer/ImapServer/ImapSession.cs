using ImapServer.Protocol;
using OpenTelemetry.Trace;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace ImapServer
{
    public class ImapSession : ImapSessionContext
    {
        private static ImapCommandParser parser = new ImapCommandParser();

        public ImapSession(TcpClient client) : base(client)
        {
        }

        public void HandleSession(CancellationToken cancellationToken)
        {
            ImapCommand.Send(this, "*", "OK IMAP4rev1 Service Ready");

            string command = string.Empty;

            try
            {
                while (!cancellationToken.IsCancellationRequested && !string.IsNullOrEmpty(command = ReadLine()))
                {
                    var cmd = parser.Parse(command);

                    if (cmd != null)
                    {
                        cmd.Execute(this, command.Split(null));
                    }
                    else
                    {
                        GenericCommand.Execute(this, "BAD", "Bad Command", command.Split(null));
                    }
                }
            }
            catch (Exception ex)
            {
                Activity.Current?.RecordException(ex);
                Activity.Current?.SetStatus(Status.Error.WithDescription(ex.Message));
            }

            this.Dispose();
        }
    }
}
