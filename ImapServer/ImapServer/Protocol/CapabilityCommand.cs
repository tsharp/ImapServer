namespace ImapServer.Protocol
{
    public class CapabilityCommand : ImapCommand
    {
        public CapabilityCommand() : base("CAPABILITY")
        {
        }

        public override bool CanParse(string command)
        {
            return base.CanParse(command);
        }

        public override void Execute(ImapSessionContext context, params string[] args)
        {
            Send(context, "*", $"CAPABILITY IMAP4rev2 {(!context.SslEnabled ? "AUTH=PLAIN LOGINDISABLED" : "STARTTLS LOGINDISABLED")}");
            Send(context, args[0], "OK CAPABILITY completed");
        }
    }
}
