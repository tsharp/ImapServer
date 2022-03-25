namespace ImapServer.Protocol
{
    public static class GenericCommand
    {
        public static void Execute(ImapSessionContext context, string command, string message, params string[] args)
        {
            ImapCommand.Send(context, args.Length > 0 ? args[0] : "*", $"{command.ToUpper()} {message}");
        }
    }
}
