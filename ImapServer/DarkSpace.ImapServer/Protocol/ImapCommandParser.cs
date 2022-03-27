using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DarkSpace.ImapServer.Protocol
{
    public class ImapCommandParser
    {
        private List<ImapCommand> commands = new List<ImapCommand>();

        private IEnumerable<Type> FindDerivedTypes<T>()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(T).IsAssignableFrom(t) && 
            !t.IsAbstract && 
            t.IsClass && 
            !t.IsGenericTypeDefinition);
        }

        private void InitializeImapCommands()
        {
            var commands = FindDerivedTypes<ImapCommand>().Select(t => (ImapCommand)Activator.CreateInstance(t));
            this.commands.AddRange(commands);
        }

        public ImapCommandParser()
        {
            InitializeImapCommands();
        }

        public ImapCommand Parse(string command)
        {
            return commands.Where(c => c.CanParse(command)).FirstOrDefault();
        }
    }
}
