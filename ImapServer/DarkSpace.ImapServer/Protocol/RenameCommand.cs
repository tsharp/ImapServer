using DarkSpace.ImapServer.Core;
using System;

namespace DarkSpace.ImapServer.Protocol
{
    public class RenameCommand : ImapCommand
    {
        public RenameCommand() : base("RENAME")
        {
        }
        protected override SessionState[] allowedStates => new[] { SessionState.Authenticated };

        public override void Execute(ImapSession session, params string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
