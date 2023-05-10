using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Text.Encodings.Web;
using System.Net.Http.Headers;
using System.Text;
using DataLayer;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace VkAppAPI.Handler
{
    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly VkApiContext vkContext;

        public BasicAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, 
            UrlEncoder encoder, ISystemClock clock, VkApiContext vkApiContext) : base(options, logger, encoder, clock)
        {
            vkContext = vkApiContext;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("");
            }

            //if (Request.Headers["Authorization"] is null) { return AuthenticateResult.Fail("Request.Headers[\"Authorization\"] is null"); }
            var headerValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (headerValue is null) { return AuthenticateResult.Fail("header value is null"); }

            if (headerValue.Parameter is null) { return AuthenticateResult.Fail("headerValue.Parametr is null"); }

            var byteArray = Convert.FromBase64String(headerValue.Parameter);
            string credentials = Encoding.UTF8.GetString(byteArray);

            if (string.IsNullOrEmpty(credentials)) { return AuthenticateResult.Fail("credentials are null or empty"); }

            string[] array = credentials.Split(":");
            string login = array[0];
            string password = array[1];

            var user = await vkContext.Users.FirstOrDefaultAsync(user => user.Login == login && user.Password == password);
            if (user is null) { return AuthenticateResult.Fail("User hasn't found"); }

            var claim = new[] { new Claim(ClaimTypes.Name, login) };
            var identity = new ClaimsIdentity(claim, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal,Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
    
}
