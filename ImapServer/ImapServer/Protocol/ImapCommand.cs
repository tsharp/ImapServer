using System.Text;
using System.Text.RegularExpressions;

namespace ImapServer.Protocol
{
    public abstract class ImapCommand
    {
        Regex rex = null;

        public ImapCommand(string command, int parameters = 0)
        {
            var parametersRex = new StringBuilder();

            for (int i = 0; i < parameters; i++)
            {
                parametersRex.Append(" ([^\\s]+)");
            }

            rex = new Regex($"^([a-zA-Z0-9]+) {command}{parametersRex}$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public virtual bool CanParse(string command)
        {
            return rex.IsMatch(command);
        }

        public abstract void Execute(ImapSessionContext context, params string[] args);

        public static void Send(ImapSessionContext context, string tag, string data)
        {
            context.WriteLine($"{tag} {data}".Trim());
        }
    }
}
