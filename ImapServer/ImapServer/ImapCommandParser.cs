using ImapServer.Protocol;
using System.Collections.Generic;
using System.Linq;

namespace ImapServer
{
    public class ImapCommandParser
    {
        private List<ImapCommand> commands = new List<ImapCommand>();

        public ImapCommandParser()
        {
            commands.Add(new AuthenticateCommand());
            commands.Add(new CapabilityCommand());
            commands.Add(new LoginCommand());
            commands.Add(new ListCommand());
            commands.Add(new StartTlsCommand());
        }

        public ImapCommand Parse(string command)
        {
            return commands.Where(c => c.CanParse(command)).FirstOrDefault();
        }
    }
}
