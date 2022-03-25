namespace ImapServer.Protocol
{
    public class ListCommand : ImapCommand
    {
        public ListCommand() : base("LIST", 2)
        {
        }

        public override void Execute(ImapSessionContext context, params string[] args)
        {
            // Send(context, "*", "LIST (\\Noselect) \"/\" \"\"");
            Send(context, "*", "LIST (\\Noselect) \"/\" INBOX");
            Send(context, args[0], "OK LIST completed");
            // Send(writer, "*", "CAPABILITY IMAP4rev1");
        }
    }
}
