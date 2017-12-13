using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImapServer.Protocol
{
    public class ListCommand : ImapCommand
    {
        public ListCommand() : base("^([a-z]|[0-9])+ LIST")
        {
        }

        public override void Execute(ImapSessionContext context, params string[] args)
        {
            Send(context, "*", "LIST (\\Noselect) \"/\" \"\"");
            Send(context, args[0], "OK LIST completed");
            // Send(writer, "*", "CAPABILITY IMAP4rev1");
        }
    }
}
