using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DarkSpace.MailService.Abstractions
{
    public class Folder : IMailItem
    {
        public long Id { get; set; }
        protected Folder parent;
        public string Name { get; set; }

        public string GetLocation()
        {
            if(parent == null)
            {
                return string.Empty;
            }

            var parentLocation = parent.GetLocation();

            if(string.IsNullOrWhiteSpace(parentLocation))
            {
                return Name;
            }

            return $"{parentLocation}/{Name}";
        }

        public Folder(string name, Folder parent = null)
        {
            this.Name = name;
            this.parent = parent;
        }

        public ConcurrentBag<IMailItem> Items { get; set; }

        public IEnumerable<Folder> GetFolders(Folder folder = null)
        {
            return (folder ?? this).Items.OfType<Folder>().ToArray();
        }

        public IEnumerable<MailMessage> GetMessages(Folder folder = null)
        {
            return (folder ?? this).Items.OfType<MailMessage>().ToArray();
        }

        public IEnumerable<MailMessage> GetAllMessages(Folder folder = null)
        {
            var nestedFolders = GetFolders(folder ?? this);

            var messages = new List<MailMessage>();
            var currentFolderMessages = GetMessages(folder ?? this);

            if (currentFolderMessages.Any())
            {
                messages.AddRange(currentFolderMessages);
            }

            foreach(var nestedFolder in nestedFolders)
            {
                var nestedMessages = GetAllMessages(nestedFolder);

                if(nestedMessages.Any())
                {
                    messages.AddRange(nestedMessages);
                }
            }

            return currentFolderMessages.ToArray();
        }
    }
}
