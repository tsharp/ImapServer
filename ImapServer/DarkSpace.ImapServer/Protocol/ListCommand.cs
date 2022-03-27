using DarkSpace.ImapServer.Core;

namespace DarkSpace.ImapServer.Protocol
{
    public class ListCommand : ImapCommand
    {
        public ListCommand() : base("LIST", 2)
        {
        }
        protected override SessionState[] allowedStates => new[] { SessionState.Authenticated };

        public override void Execute(ImapSession session, params string[] args)
        {
            var unknown = args[2];
            var folderName = args[3];
            
            Send(session, "*", $"LIST (\\Subscribed) {unknown} {folderName}");
            Send(session, args[0], "OK LIST completed");
        }
    }
}
