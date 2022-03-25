namespace ImapServer.Protocol
{
    public class StartTlsCommand : ImapCommand
    {
        public StartTlsCommand() : base("STARTTLS")
        {
        }

        public override void Execute(ImapSessionContext context, params string[] args)
        {
            Send(context, args[0], "OK Begin TLS negotiation now");
            context.UpgradeToSsl().Wait();
        }
    }
}
