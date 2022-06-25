using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ExampleApp.Custom
{
    public class CustomAuthentication
    {
        private readonly RequestDelegate _next;
        private const string _cookieKey = "authUser";

        public CustomAuthentication(RequestDelegate requestDelegate) => _next = requestDelegate;

        public async Task Invoke(HttpContext context)
        {
            var user = context.Request.Cookies[_cookieKey];

            if (!string.IsNullOrEmpty(user))
            {
                var claim = new Claim(ClaimTypes.Name, user);
                var claimIdentity = new ClaimsIdentity("QueryStringValue");

                claimIdentity.AddClaim(claim);

                context.User = new ClaimsPrincipal(claimIdentity);
            }

            await _next(context);
        }
    }
}
