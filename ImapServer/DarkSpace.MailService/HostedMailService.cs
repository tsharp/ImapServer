using DarkSpace.MailService.Abstractions;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace DarkSpace.ImapServer
{
    internal class HostedMailService : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
