using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImapServer
{
    public class ImapCommandParser
    {
        private List<Commands.ImapCommand> _commands = new List<Commands.ImapCommand>();

        public ImapCommandParser()
        {
            _commands.Add(new Commands.CapabilityCommand());
            _commands.Add(new Commands.LoginCommand());
            _commands.Add(new Commands.ListCommand());
            _commands.Add(new Commands.StartTlsCommand());
        }

        public Commands.ImapCommand Parse(string command)
        {
            return _commands.Where(c => c.Matches(command)).FirstOrDefault();
        }
    }
}
