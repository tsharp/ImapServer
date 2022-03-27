using System.Security.Claims;

namespace DarkSpace.MailService.Abstractions
{
    public interface IMailService
    {
        ClaimsPrincipal AuthenticateUser(string userName, string password);
    }
}