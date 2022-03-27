using DarkSpace.MailService.Abstractions;
using System.Security.Claims;

namespace DarkSpace.MailService
{
    public class MailService : IMailService
    {
        public ClaimsPrincipal AuthenticateUser(string userName, string password)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Upn, userName)
            };

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "PlainAuth")
            {

            });
        }
    }
}
