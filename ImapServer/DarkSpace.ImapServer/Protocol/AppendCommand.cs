using DarkSpace.ImapServer.Core;
using System;
using System.Collections.Generic;

namespace DarkSpace.ImapServer.Protocol
{
    public class AppendCommand : ImapCommand
    {
        public AppendCommand() : base("APPEND", -1)
        {
        }

        protected override SessionState[] allowedStates => new[] { SessionState.Authenticated };

        public override void Execute(ImapSession session, params string[] args)
        {
            bool finishCommand = false;
            Send(session, "+", "Ready for literal data");
            //Send(session, "*", "OK [UIDVALIDITY 0] UIDs valid");        // TODO: UIDVALIDITY
            //Send(session, "*", "OK [UIDNEXT 1] Predicted next UID");    // TODO: UIDNEXT
            //Send(session, "*", $"LIST () \"/\" {mailboxName}");
            //Send(session, "*", "FLAGS (\\Answered \\Flagged \\Deleted \\Seen \\Draft)");
            //Send(session, "*", "OK [PERMANENTFLAGS ()] No permanent flags permitted");
            //Send(session, args[0], "OK [READ-ONLY] EXAMINE completed");
            var headers = new List<string>();

            // Headers
            while (!finishCommand)
            {
                var header = session.ReadLine();
                var command = session.Parser.Parse(header);

                if(command != null && command.GetType() == typeof(NoOpCommand))
                {
                    finishCommand = true;
                    break;
                }
                
                if (string.IsNullOrWhiteSpace(header)) 
                {
                    break;
                }

                headers.Add(header);
            }
            
            Send(session, "+", "Ready for literal data");

            // Body
            while (!finishCommand)
            {
                var body = session.ReadLine();

                var command = session.Parser.Parse(body);

                if (command != null && command.GetType() == typeof(NoOpCommand))
                {
                    finishCommand = true;
                    break;
                }

                if (string.IsNullOrWhiteSpace(body))
                {
                    break;
                }
            }

            // Ok
            Send(session, args[0], "OK APPEND completed");
        }
    }
}
