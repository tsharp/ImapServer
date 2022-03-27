using DarkSpace.ImapServer.Core;

namespace DarkSpace.ImapServer.Protocol
{
    public class CapabilityCommand : ImapCommand
    {
        public CapabilityCommand() : base("CAPABILITY")
        {
        }

        protected override SessionState[] allowedStates => new[] { SessionState.NotAuthenticated, SessionState.Authenticated, SessionState.Selected };

        public override bool CanParse(string command)
        {
            return base.CanParse(command);
        }

        public override void Execute(ImapSession session, params string[] args)
        {
            Send(session, "*", $"CAPABILITY IMAP4rev2 {(!session.SslEnabled ? "AUTH=PLAIN LOGINDISABLED" : "STARTTLS LOGINDISABLED")}");
            Send(session, args[0], "OK CAPABILITY completed");
        }
    }
}
