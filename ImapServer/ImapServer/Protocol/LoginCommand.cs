namespace ImapServer.Protocol
{
    public class LoginCommand : ImapCommand
    {
        public LoginCommand() : base("LOGIN", 2)
        {
        }

        public override void Execute(ImapSessionContext context, params string[] args)
        {
            GenericCommand.Execute(context, "OK", "Login Success", args);
        }
    }
}
