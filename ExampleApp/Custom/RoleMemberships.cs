using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ExampleApp.Custom
{
    public class RoleMemberships
    {
        private readonly RequestDelegate _next;

        public RoleMemberships(RequestDelegate requestDelegate) => _next = requestDelegate;

        public async Task Invoke(HttpContext context)
        {
            var mainIdentity = context.User.Identity;

            if (mainIdentity.IsAuthenticated && UsersAndClaims.Claims.ContainsKey(mainIdentity.Name))
            {
                var identity = new ClaimsIdentity("Roles");

                identity.AddClaim(new Claim(ClaimTypes.Name, mainIdentity.Name));
                identity.AddClaims(UsersAndClaims.Claims[mainIdentity.Name]);

                context.User.AddIdentity(identity);
            }

            await _next(context);
        }
    }
}
