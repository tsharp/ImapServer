using DarkSpace.ImapServer.Core;
using System;

namespace DarkSpace.ImapServer.Protocol
{
    public class ExamineCommand : ImapCommand
    {
        public ExamineCommand() : base("EXAMINE", 1)
        {
        }
        protected override SessionState[] allowedStates => new[] { SessionState.Authenticated };

        public override void Execute(ImapSession session, params string[] args)
        {
            var mailboxName = args[2];
            Send(session, "*", "{sendCount} EXISTS");
            Send(session, "*", "OK [UIDVALIDITY 0] UIDs valid");        // TODO: UIDVALIDITY
            Send(session, "*", "OK [UIDNEXT 1] Predicted next UID");    // TODO: UIDNEXT
            Send(session, "*", $"LIST () \"/\" {mailboxName}");
            Send(session, "*", "FLAGS (\\Answered \\Flagged \\Deleted \\Seen \\Draft)");
            Send(session, "*", "OK [PERMANENTFLAGS ()] No permanent flags permitted");
            Send(session, args[0], "OK [READ-ONLY] EXAMINE completed");
        }
    }
}
