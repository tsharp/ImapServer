using DarkSpace.ImapServer.Core;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DarkSpace.ImapServer.Protocol
{
    public abstract class ImapCommand
    {
        Regex rex = null;

        protected abstract SessionState[] allowedStates { get; }

        public ImapCommand(string command, int parameters = 0)
        {
            var parametersRex = new StringBuilder();

            if (parameters > 0)
            {
                for (int i = 0; i < parameters; i++)
                {
                    parametersRex.Append(" ([^\\s]+)");
                }
            }
            // One or more ...
            else if (parameters == -1)
            {
                parametersRex.Append(" (.*)");
            }

            rex = new Regex($"^([a-zA-Z0-9]+) {command}{parametersRex}$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public virtual bool CanParse(string command)
        {
            if(command == null)
            {
                return false;
            }

            return rex.IsMatch(command);
        }

        internal void InternalExecute(ImapSession session, params string[] args)
        {
            if (!allowedStates.Contains((session as ImapSession).GetState()))
            {
                throw new System.Exception("Invalid State ...");
                return;
            }

            Execute(session, args);
        }

        public abstract void Execute(ImapSession session, params string[] args);

        public static void Send(ImapSession session, string tag, string data)
        {
            session.WriteLine($"{tag} {data}".Trim());
        }
    }
}
