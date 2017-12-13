using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace ImapServer.CoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ImapServer server = new ImapServer();
            Task.WaitAll(server.StartAsync(CancellationToken.None));
        }
    }
}
