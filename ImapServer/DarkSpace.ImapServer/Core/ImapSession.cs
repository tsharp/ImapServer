using DarkSpace.ImapServer.Protocol;
using DarkSpace.MailService.Abstractions;
using OpenTelemetry.Trace;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace DarkSpace.ImapServer.Core
{
    public class ImapSession : ImapSessionContext
    {
        public readonly ImapCommandParser Parser = new ImapCommandParser();
        private string selectedNailboxName = string.Empty;

        public ImapSession(TcpClient client, IMailService mailService) : base(client, mailService)
        {
        }

        public bool Select(string mailboxName)
        {
            bool wasPreviouslySelected = sessionState == SessionState.Selected;

            selectedNailboxName = mailboxName;
            SetState(SessionState.Selected);

            if (wasPreviouslySelected)
            {
                WriteLine("* OK [CLOSED] Previous mailbox is now closed");
            }

            return true;
        }

        private SessionState sessionState = SessionState.NotAuthenticated;

        internal void SetState(SessionState sessionState)
        {
            if (this.sessionState != SessionState.Closed)
            {
                this.sessionState = sessionState;
            }
        }

        internal void End()
        {
            SetState(SessionState.Closed);
        }

        internal SessionState GetState()
        {
            return sessionState;
        }

        public void HandleSession(CancellationToken cancellationToken)
        {
            ImapCommand.Send(this, "*", "OK IMAP4rev2 Service Ready");

            string command = string.Empty;

            try
            {
                while (sessionState != SessionState.Closed && 
                    !cancellationToken.IsCancellationRequested && 
                    !string.IsNullOrEmpty(command = ReadLine()))
                {
                    var cmd = Parser.Parse(command);

                    if (cmd != null)
                    {
                        cmd.Execute(this, command.Split(null));
                    }
                    else
                    {
                        GenericCommand.Execute(this, "BAD", "Bad Command", command.Split(null));
                    }
                }
            }
            catch (Exception ex)
            {
                Activity.Current?.RecordException(ex);
                Activity.Current?.SetStatus(Status.Error.WithDescription(ex.Message));
            }

            this.Dispose();
        }
    }
}
