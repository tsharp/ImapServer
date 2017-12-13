using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImapServer.Protocol
{
    public class CapabilityCommand : ImapCommand
    {
        public CapabilityCommand() : base("^([a-z]|[0-9])+ CAPABILITY$")
        {
        }

        public override void Execute(ImapSessionContext context, params string[] args)
        {
            Send(context, "*", $"CAPABILITY IMAP4rev1 {(context.SslEnabled ? "AUTH=PLAIN" : "STARTTLS LOGINDISABLED")}");
            Send(context, args[0], "OK CAPABILITY completed");
        }
    }
}
