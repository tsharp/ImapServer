using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImapServer.Protocol
{
    public class LoginCommand : ImapCommand
    {
        public LoginCommand() : base("^([a-z]|[0-9])+ LOGIN (.*) (.*)$")
        {
        }

        public override void Execute(ImapSessionContext context, params string[] args)
        {
            GenericCommand.Execute(context, "OK", "Login Success", args);
        }
    }
}
