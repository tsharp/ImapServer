using DarkSpace.ImapServer.Core;
using System;

namespace DarkSpace.ImapServer.Protocol
{
    public class SelectCommand : ImapCommand
    {
        public SelectCommand() : base("SELECT", 1)
        {
        }
        protected override SessionState[] allowedStates => new[] { SessionState.Authenticated, SessionState.Selected };

        public override void Execute(ImapSession session, params string[] args)
        {
            var mailboxName = args[2];
            session.Select(mailboxName);
            Send(session, "*", "{sendCount} EXISTS");
            Send(session, "*", "OK [UIDVALIDITY 0] UIDs valid");        // TODO: UIDVALIDITY
            Send(session, "*", "OK [UIDNEXT 1] Predicted next UID");    // TODO: UIDNEXT
            Send(session, "*", $"LIST () \"/\" {mailboxName}");
            Send(session, "*", "FLAGS (\\Answered \\Flagged \\Deleted \\Seen \\Draft)");
            Send(session, "*", "OK [PERMANENTFLAGS (\\Deleted \\Seen \\Answered \\Flagged \\Draft \\*)] System flags and keywords allowed");
            Send(session, args[0], "OK [READ-WRITE] SELECT completed");
        }
    }
}
