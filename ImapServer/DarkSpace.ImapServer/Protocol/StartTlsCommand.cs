using DarkSpace.ImapServer.Core;

namespace DarkSpace.ImapServer.Protocol
{
    public class StartTlsCommand : ImapCommand
    {
        public StartTlsCommand() : base("STARTTLS")
        {
        }
        protected override SessionState[] allowedStates => new[] { SessionState.Authenticated };

        public override void Execute(ImapSession session, params string[] args)
        {
            Send(session, args[0], "OK Begin TLS negotiation now");
            session.UpgradeToSsl().Wait();
        }
    }
}
