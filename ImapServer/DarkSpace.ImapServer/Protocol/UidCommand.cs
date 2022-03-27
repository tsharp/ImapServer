using DarkSpace.ImapServer.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DarkSpace.ImapServer.Protocol
{
    public class UidCommand : ImapCommand
    {
        public UidCommand() : base("UID", -1)
        {
        }
        protected override SessionState[] allowedStates => new[] { SessionState.Authenticated, SessionState.Selected };

        public override void Execute(ImapSession session, params string[] args)
        {
            if (args[2] == "SEARCH" && args[3] == "ALL")
            {
                var results = new List<long>();
                
                for(var i = 0; i < 10000; i++)
                {
                    results.Add(i);
                }

                Send(session, "*", $"UID SEARCH {string.Join(",", results)}");
            }

            Send(session, args[0], "OK SEARCH completed");
        }
    }
}
