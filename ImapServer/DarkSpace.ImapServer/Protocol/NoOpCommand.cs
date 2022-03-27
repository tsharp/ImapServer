using DarkSpace.ImapServer.Core;
using System;

namespace DarkSpace.ImapServer.Protocol
{
    public class NoOpCommand : ImapCommand
    {
        public NoOpCommand() : base("NOOP")
        {
        }

        protected override SessionState[] allowedStates => new[] { SessionState.NotAuthenticated, SessionState.Authenticated, SessionState.Selected };

        public override void Execute(ImapSession session, params string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
