using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ImapServer.Commands;
using Serilog;

namespace ImapServer
{
    public class ImapSession : ImapSessionContext
    {
        private static ImapCommandParser _parser = new ImapCommandParser();
        
        public ImapSession(TcpClient client) : base(client)
        {
        }

        public void HandleSession(CancellationToken cancellationToken)
        {
            ImapCommand.Send(this, "*", "OK IMAP4rev1 Service Ready");

            string command = string.Empty;

            try
            {
                while (!String.IsNullOrEmpty(command = ReadLine()))
                {
                    var cmd = _parser.Parse(command);

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
            catch(Exception ex)
            {
                Log.Error(ex, "Error");
            }

            this.Dispose();
        }
    }
}
