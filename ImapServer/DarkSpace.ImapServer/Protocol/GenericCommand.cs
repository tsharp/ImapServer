using DarkSpace.ImapServer.Core;

namespace DarkSpace.ImapServer.Protocol
{
    public static class GenericCommand
    {
        public static void Execute(ImapSession session, string command, string message, params string[] args)
        {
            ImapCommand.Send(session, args.Length > 0 ? args[0] : "*", $"{command.ToUpper()} {message}");
        }
    }
}
