using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImapServer.Protocol
{
    public abstract class ImapCommand
    {
        Regex rex = null;
        
        public ImapCommand(string command)
        {
            rex = new Regex($"^([a-z]|[0-9])+ {command}$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public bool CanParse(string command)
        {
            return rex.IsMatch(command);
        }

        public abstract void Execute(ImapSessionContext context, params string[] args);

        public static void Send(ImapSessionContext context, string tag, string data)
        {
            context.WriteLine($"{tag} {data}");
        }
    }
}
