using DarkSpace.ImapServer.Core;
using System;

namespace DarkSpace.ImapServer.Protocol
{
    public class LogoutCommand : ImapCommand
    {
        public LogoutCommand() : base("LOGOUT")
        {
        }

        protected override SessionState[] allowedStates => new[] { SessionState.NotAuthenticated, SessionState.Authenticated, SessionState.Selected };

        public override void Execute(ImapSession session, params string[] args)
        {
            Send(session, "*", "BYE IMAP4rev2 Server logging out");
            Send(session, args[0], "OK LOGOUT completed");
            session.End();
        }
    }
}
