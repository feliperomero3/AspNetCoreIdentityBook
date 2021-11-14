using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ExampleApp.Custom
{
    public static class CustomSignInAndSignOut
    {
        private const string cookieKey = "authUser";

        public static async Task SignIn(HttpContext context)
        {
            string user = context.Request.Query["user"];

            if (user != null)
            {
                var claim = new Claim(ClaimTypes.Name, user);
                var identity = new ClaimsIdentity(ExampleAppConstants.Scheme);

                identity.AddClaim(claim);

                await context.SignInAsync(new ClaimsPrincipal(identity));

                await context.Response.WriteAsync($"Authenticated user: {user}");
            }
            else
            {
                await context.ChallengeAsync();
            }
        }

        public static async Task SignOut(HttpContext context)
        {
            await context.SignOutAsync();

            await context.Response.WriteAsync("Signed out");
        }
    }
}
