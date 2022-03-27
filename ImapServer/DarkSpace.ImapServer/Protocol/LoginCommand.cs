using DarkSpace.ImapServer.Core;

namespace DarkSpace.ImapServer.Protocol
{
    public class LoginCommand : ImapCommand
    {
        public LoginCommand() : base("LOGIN", 2)
        {
        }
        protected override SessionState[] allowedStates => new[] { SessionState.NotAuthenticated };

        public override void Execute(ImapSession session, params string[] args)
        {
            GenericCommand.Execute(session, "OK", "Login Success", args);
        }
    }
}
