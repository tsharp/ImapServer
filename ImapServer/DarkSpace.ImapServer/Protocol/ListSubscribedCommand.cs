﻿using DarkSpace.ImapServer.Core;
using System;

namespace DarkSpace.ImapServer.Protocol
{
    public class ListSubscribedCommand : ImapCommand
    {
        public ListSubscribedCommand() : base("LSUB")
        {
        }
        protected override SessionState[] allowedStates => new[] { SessionState.Authenticated };

        public override void Execute(ImapSession session, params string[] args)
        {
            throw new NotImplementedException();
        }
    }
}