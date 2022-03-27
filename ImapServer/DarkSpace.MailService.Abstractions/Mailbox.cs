using System;

namespace DarkSpace.MailService.Abstractions
{

    public class Mailbox : Folder
    {
        public Mailbox() : base("")
        {
            this.Items.Add(new Folder("Inbox", this));
            this.Items.Add(new Folder("Drafts", this));
            this.Items.Add(new Folder("Sent", this));
            this.Items.Add(new Folder("Deleted", this));
        }

        public Guid Id { get; set; }
        public string EmailAddress { get; set; }
    }
}
