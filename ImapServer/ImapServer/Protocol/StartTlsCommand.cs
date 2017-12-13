using System;
using System.Collections.Generic;
using System.Text;

namespace ImapServer.Protocol
{
    public class StartTlsCommand : ImapCommand
    {
        public StartTlsCommand() : base("^([a-z]|[0-9])+ STARTTLS")
        {
        }

        public override void Execute(ImapSessionContext context, params string[] args)
        {
            Send(context, args[0], "OK Begin TLS negotiation now");
            context.UpgradeToSsl().Wait();
        }
    }
}
